-- ============================================
-- Fix: Comentarios en Trazabilidad
-- Fecha: 2024-12-30
-- Descripción: Actualiza el trigger para que NO muestre el texto del comentario
-- ============================================

-- Actualizar la función del trigger de comentarios
CREATE OR REPLACE FUNCTION trg_comentarios_trazabilidad_func()
RETURNS TRIGGER AS $$
BEGIN
    -- Registrar en trazabilidad sin incluir el texto del comentario
    INSERT INTO trazabilidad_solicitudes (id_solicitud, id_usuario_actor, accion, descripcion, fecha_evento)
    VALUES (
        NEW.id_solicitud,
        NEW.id_usuario,
        'Comentario Agregado',
        'Se agregó un nuevo comentario a la solicitud',  -- Sin incluir el texto
        NOW()
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Verificar que el trigger existe
SELECT 
    trigger_name,
    event_object_table,
    action_timing || ' ' || event_manipulation AS evento,
    CASE 
        WHEN trigger_name IS NOT NULL THEN '? Trigger activo'
        ELSE '? Trigger no encontrado'
    END AS estado
FROM information_schema.triggers
WHERE trigger_name = 'trg_comentarios_trazabilidad';

-- Mensaje de confirmación
DO $$
BEGIN
    RAISE NOTICE 'Trigger actualizado correctamente';
    RAISE NOTICE 'Ahora la trazabilidad mostrará: "Se agregó un nuevo comentario a la solicitud"';
    RAISE NOTICE 'El texto completo del comentario sigue visible en la sección de comentarios';
END $$;
