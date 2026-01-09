# Sistema de Gestion de Tickets - Backend

Sistema de gestion de solicitudes internas (Mesa de Servicios) desarrollado con .NET 10 y PostgreSQL. Implementa una arquitectura limpia de 4 capas para la administracion de tickets departamentales.

## Descripcion

API REST para la gestion integral de solicitudes internas empresariales. Permite a los empleados crear tickets de soporte dirigidos a diferentes areas (TI, RRHH, Logistica, Contabilidad, etc.), con seguimiento completo desde su creacion hasta su resolucion.

## Arquitectura

El proyecto implementa Clean Architecture con separacion de responsabilidades:

```
SistemaTikets/
??? SistemaTikets.Domain/          # Entidades y contratos de repositorio
??? SistemaTikets.Application/     # Logica de negocio, servicios y DTOs
??? SistemaTikets.Infrastructure/  # Persistencia, seguridad y repositorios
??? SistemaTikets.WebApi/          # Controllers y configuracion de API
```

## Tecnologias

- **.NET 10** - Framework principal
- **PostgreSQL** - Base de datos relacional
- **Entity Framework Core** - ORM para acceso a datos
- **JWT (JSON Web Tokens)** - Autenticacion y autorizacion
- **Swagger/OpenAPI** - Documentacion de API
- **SHA256** - Hash de contrasenas

## Caracteristicas Principales

### Gestion de Usuarios
- 3 roles: Administrador, Gestor y Solicitante
- Autenticacion mediante JWT
- Gestion de usuarios por administradores
- Asignacion de areas a usuarios

### Gestion de Solicitudes
- Creacion de tickets con archivos adjuntos
- Estados: Nueva, En Progreso, Resuelta, Cerrada, Cancelada
- Prioridades: Baja, Media, Alta, Critica
- Asignacion manual o automatica de gestores
- Edicion de solicitudes en estado "Nueva"

### Sistema de Areas
- Multiples areas predefinidas: TI, RRHH, Logistica, Contabilidad, Operaciones, Ventas, Marketing
- Tipos de solicitud especificos por area
- Gestores encargados por area

### Trazabilidad
- Registro completo de cambios de estado
- Sistema de comentarios en solicitudes
- Historial de asignaciones

### Gestion de Archivos
- Subida de archivos adjuntos a solicitudes
- Descarga y visualizacion de documentos
- Soporte para PDF, imagenes, Word, Excel

## Prerequisitos

- .NET 10 SDK
- PostgreSQL 12 o superior
- Git

## Instalacion

1. Clonar el repositorio:
```bash
git clone https://github.com/DOOM-EXE/SistemaTikets-Backend.git
cd SistemaTikets-Backend/SistemaTikets
```

2. Configurar la cadena de conexion en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=sistema_tickets;Username=postgres;Password=tu_password"
  }
}
```

3. Crear la base de datos:
```bash
dotnet ef database update --project SistemaTikets.Infrastructure --startup-project SistemaTikets.WebApi
```

4. Ejecutar el proyecto:
```bash
cd SistemaTikets.WebApi
dotnet run
```

La API estara disponible en `https://localhost:7000` y `http://localhost:5000`

## Datos Iniciales (Seeding)

El sistema incluye datos de prueba que se cargan automaticamente al iniciar:

### Usuarios de Prueba

**Administradores:**
- Username: `superadmin` / Password: `SuperAdmin123!`
- Username: `admin` / Password: `Admin123!`

**Gestores por Area:**
- TI: `gestor.ti` / `Gestor123!`
- RRHH: `gestor.rrhh` / `Gestor123!`
- Logistica: `gestor.logistica` / `Gestor123!`
- Contabilidad: `gestor.contabilidad` / `Gestor123!`

**Solicitantes:**
- Username: `jperez` / Password: `User123!`
- Username: `lmartinez` / Password: `User123!`

## Endpoints Principales

### Autenticacion
- `POST /api/Auth/login` - Iniciar sesion

### Usuarios
- `GET /api/Usuarios` - Listar usuarios
- `POST /api/Usuarios` - Crear usuario
- `PUT /api/Usuarios/{id}` - Actualizar usuario
- `POST /api/Usuarios/{id}/cambiar-estado` - Activar/desactivar usuario

### Solicitudes
- `GET /api/Solicitudes` - Listar solicitudes
- `GET /api/Solicitudes/{id}` - Detalle de solicitud
- `POST /api/Solicitudes` - Crear solicitud
- `PUT /api/Solicitudes/{id}` - Editar solicitud
- `POST /api/Solicitudes/{id}/cambiar-estado` - Cambiar estado
- `POST /api/Solicitudes/{id}/asignar-gestor` - Asignar gestor
- `POST /api/Solicitudes/{id}/tomar` - Gestor toma solicitud

### Comentarios
- `GET /api/Comentarios/solicitud/{idSolicitud}` - Comentarios de solicitud
- `POST /api/Comentarios` - Agregar comentario

### Catalogos
- `GET /api/Catalogos/areas` - Listar areas
- `GET /api/Catalogos/tipos-solicitud` - Tipos de solicitud
- `GET /api/Catalogos/prioridades` - Prioridades
- `GET /api/Catalogos/estados` - Estados
- `GET /api/Catalogos/roles` - Roles

### Archivos
- `POST /api/Archivos/upload` - Subir archivo
- `GET /api/Archivos/download/{fileName}` - Descargar archivo
- `GET /api/Archivos/view/{fileName}` - Visualizar archivo

## Documentacion de API

Una vez ejecutado el proyecto, accede a la documentacion interactiva Swagger en:
```
http://localhost:5000/swagger
```

## Estructura de Base de Datos

### Tablas Principales
- **Usuarios** - Informacion de usuarios del sistema
- **Roles** - Roles de usuario (Admin, Gestor, Solicitante)
- **Areas** - Departamentos de la empresa
- **Solicitudes** - Tickets de soporte
- **Estados** - Estados de solicitudes
- **Prioridades** - Niveles de prioridad
- **TiposSolicitud** - Categorias de solicitudes por area
- **Comentarios** - Comentarios en solicitudes
- **TrazabilidadSolicitud** - Historial de cambios
- **Encargados** - Gestores asignados a areas

## Configuracion JWT

El sistema utiliza JWT para autenticacion con las siguientes configuraciones:

- **Tiempo de expiracion**: 480 minutos (8 horas)
- **Algoritmo**: HS256
- **Claims**: UserId, Username, Role, IdArea

## CORS

Configurado para aceptar peticiones desde:
- `http://localhost:5173`
- `http://localhost:5174`
- `http://localhost:3000`

## Seguridad

- Autenticacion mediante JWT Bearer Token
- Hash de contrasenas con SHA256
- Autorizacion basada en roles
- Validacion de permisos por endpoint

## Flujo de Trabajo de Solicitudes

1. **Solicitante** crea una solicitud con adjuntos
2. Sistema genera codigo unico y asigna estado "Nueva"
3. **Encargado de Area** o **Gestor** puede asignar la solicitud
4. **Gestor asignado** cambia estado a "En Progreso"
5. Gestor agrega comentarios y actualiza la solicitud
6. Gestor marca como "Resuelta"
7. Sistema automaticamente cierra solicitudes resueltas mayores a 7 dias

## Migraciones

Para crear una nueva migracion:
```bash
dotnet ef migrations add NombreMigracion --project SistemaTikets.Infrastructure --startup-project SistemaTikets.WebApi
```

Para aplicar migraciones:
```bash
dotnet ef database update --project SistemaTikets.Infrastructure --startup-project SistemaTikets.WebApi
```

## Contribucion

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/NuevaFuncionalidad`)
3. Commit tus cambios (`git commit -m 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/NuevaFuncionalidad`)
5. Abre un Pull Request

## Licencia

Este proyecto es de uso interno empresarial.

## Contacto

Repositorio: [https://github.com/DOOM-EXE/SistemaTikets-Backend](https://github.com/DOOM-EXE/SistemaTikets-Backend)
