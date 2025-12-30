-- ============================================
-- Migración: Agregar Tabla Encargados y Triggers para PostgreSQL
-- Fecha: 2024-12-30
-- Descripción: 
--   1. Crea la tabla encargados para gestionar múltiples encargados por área
--   2. Crea triggers para trazabilidad automática en comentarios
--   3. Crea triggers para trazabilidad automática en cambios de estado
-- ============================================

-- ==============================================
-- PARTE 1: TRIGGER PARA COMENTARIOS
-- ==============================================

-- Función para trigger de comentarios
CREATE OR REPLACE FUNCTION trg_comentarios_trazabilidad_func()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO trazabilidad_solicitudes (id_solicitud, id_usuario_actor, accion, descripcion, fecha_evento)
    VALUES (
        NEW.id_solicitud,
        NEW.id_usuario,
        'Comentario Agregado',
        'Se agregó un nuevo comentario: ' || LEFT(NEW.texto, 100) || CASE WHEN LENGTH(NEW.texto) > 100 THEN '...' ELSE '' END,
        NOW()
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Crear trigger para comentarios
DROP TRIGGER IF EXISTS trg_comentarios_trazabilidad ON comentarios;
CREATE TRIGGER trg_comentarios_trazabilidad
    AFTER INSERT ON comentarios
    FOR EACH ROW
    EXECUTE FUNCTION trg_comentarios_trazabilidad_func();

COMMENT ON FUNCTION trg_comentarios_trazabilidad_func() IS 'Trigger que registra automáticamente en la trazabilidad cuando se agrega un comentario a una solicitud.';

-- ==============================================
-- PARTE 2: TRIGGER PARA CAMBIOS DE ESTADO
-- ==============================================

-- Función para trigger de cambio de estado
CREATE OR REPLACE FUNCTION trg_solicitudes_cambio_estado_func()
RETURNS TRIGGER AS $$
DECLARE
    v_estado_old VARCHAR(50);
    v_estado_new VARCHAR(50);
BEGIN
    -- Solo registrar si cambió el estado
    IF NEW.id_estado IS DISTINCT FROM OLD.id_estado THEN
        -- Obtener nombres de estados
        SELECT nombre INTO v_estado_old FROM estados WHERE id_estado = OLD.id_estado;
        SELECT nombre INTO v_estado_new FROM estados WHERE id_estado = NEW.id_estado;
        
        INSERT INTO trazabilidad_solicitudes (id_solicitud, id_usuario_actor, accion, descripcion, fecha_evento)
        VALUES (
            NEW.id_solicitud,
            COALESCE(NEW.id_gestor_asignado, NEW.id_solicitante),
            'Cambio de Estado',
            'Estado cambiado de "' || v_estado_old || '" a "' || v_estado_new || '"',
            NOW()
        );
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Crear trigger para cambio de estado
DROP TRIGGER IF EXISTS trg_solicitudes_cambio_estado ON solicitudes;
CREATE TRIGGER trg_solicitudes_cambio_estado
    AFTER UPDATE ON solicitudes
    FOR EACH ROW
    EXECUTE FUNCTION trg_solicitudes_cambio_estado_func();

COMMENT ON FUNCTION trg_solicitudes_cambio_estado_func() IS 'Trigger que registra automáticamente en la trazabilidad cuando cambia el estado de una solicitud.';

-- ==============================================
-- PARTE 3: TRIGGER PARA ASIGNACIÓN DE GESTOR
-- ==============================================

-- Función para trigger de asignación de gestor
CREATE OR REPLACE FUNCTION trg_solicitudes_asignacion_gestor_func()
RETURNS TRIGGER AS $$
DECLARE
    v_nombre_old VARCHAR(150);
    v_nombre_new VARCHAR(150);
    v_descripcion TEXT;
BEGIN
    -- Solo registrar si cambió el gestor asignado
    IF NEW.id_gestor_asignado IS DISTINCT FROM OLD.id_gestor_asignado THEN
        -- Obtener nombres de usuarios
        IF OLD.id_gestor_asignado IS NOT NULL THEN
            SELECT nombre_completo INTO v_nombre_old FROM usuarios WHERE id_usuario = OLD.id_gestor_asignado;
        END IF;
        
        IF NEW.id_gestor_asignado IS NOT NULL THEN
            SELECT nombre_completo INTO v_nombre_new FROM usuarios WHERE id_usuario = NEW.id_gestor_asignado;
        END IF;
        
        -- Determinar descripción según el caso
        IF OLD.id_gestor_asignado IS NULL AND NEW.id_gestor_asignado IS NOT NULL THEN
            v_descripcion := 'Solicitud asignada a ' || v_nombre_new;
        ELSIF OLD.id_gestor_asignado IS NOT NULL AND NEW.id_gestor_asignado IS NULL THEN
            v_descripcion := 'Asignación removida de ' || v_nombre_old;
        ELSE
            v_descripcion := 'Reasignada de ' || v_nombre_old || ' a ' || v_nombre_new;
        END IF;
        
        INSERT INTO trazabilidad_solicitudes (id_solicitud, id_usuario_actor, accion, descripcion, fecha_evento)
        VALUES (
            NEW.id_solicitud,
            COALESCE(NEW.id_asignado_por, NEW.id_solicitante),
            'Asignación de Gestor',
            v_descripcion,
            NOW()
        );
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Crear trigger para asignación de gestor
DROP TRIGGER IF EXISTS trg_solicitudes_asignacion_gestor ON solicitudes;
CREATE TRIGGER trg_solicitudes_asignacion_gestor
    AFTER UPDATE ON solicitudes
    FOR EACH ROW
    EXECUTE FUNCTION trg_solicitudes_asignacion_gestor_func();

COMMENT ON FUNCTION trg_solicitudes_asignacion_gestor_func() IS 'Trigger que registra automáticamente en la trazabilidad cuando se asigna o reasigna un gestor a una solicitud.';

-- ==============================================
-- PARTE 4: TRIGGER PARA CAMBIOS DE PRIORIDAD
-- ==============================================

-- Función para trigger de cambio de prioridad
CREATE OR REPLACE FUNCTION trg_solicitudes_cambio_prioridad_func()
RETURNS TRIGGER AS $$
DECLARE
    v_prioridad_old VARCHAR(50);
    v_prioridad_new VARCHAR(50);
BEGIN
    -- Solo registrar si cambió la prioridad
    IF NEW.id_prioridad IS DISTINCT FROM OLD.id_prioridad THEN
        -- Obtener nombres de prioridades
        SELECT nombre INTO v_prioridad_old FROM prioridades WHERE id_prioridad = OLD.id_prioridad;
        SELECT nombre INTO v_prioridad_new FROM prioridades WHERE id_prioridad = NEW.id_prioridad;
        
        INSERT INTO trazabilidad_solicitudes (id_solicitud, id_usuario_actor, accion, descripcion, fecha_evento)
        VALUES (
            NEW.id_solicitud,
            COALESCE(NEW.id_gestor_asignado, NEW.id_solicitante),
            'Cambio de Prioridad',
            'Prioridad cambiada de "' || v_prioridad_old || '" a "' || v_prioridad_new || '"',
            NOW()
        );
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Crear trigger para cambio de prioridad
DROP TRIGGER IF EXISTS trg_solicitudes_cambio_prioridad ON solicitudes;
CREATE TRIGGER trg_solicitudes_cambio_prioridad
    AFTER UPDATE ON solicitudes
    FOR EACH ROW
    EXECUTE FUNCTION trg_solicitudes_cambio_prioridad_func();

COMMENT ON FUNCTION trg_solicitudes_cambio_prioridad_func() IS 'Trigger que registra automáticamente en la trazabilidad cuando cambia la prioridad de una solicitud.';

-- ==============================================
-- VERIFICACIÓN
-- ==============================================

-- Verificar que la tabla encargados existe
SELECT 
    table_name,
    column_name,
    data_type,
    is_nullable
FROM information_schema.columns
WHERE table_name = 'encargados'
ORDER BY ordinal_position;

-- Verificar que los triggers se crearon correctamente
SELECT 
    trigger_name,
    event_object_table AS tabla_asociada,
    action_timing || ' ' || event_manipulation AS evento
FROM information_schema.triggers
WHERE event_object_table IN ('comentarios', 'solicitudes')
ORDER BY event_object_table, trigger_name;

-- Verificar encargados (debería estar vacío al inicio)
SELECT COUNT(*) as total_encargados FROM encargados;

\echo 'Migración completada exitosamente.'
\echo 'Tabla "encargados" ya existe (creada por Entity Framework).'
\echo 'Triggers de trazabilidad creados para comentarios, estados, asignaciones y prioridades.'
