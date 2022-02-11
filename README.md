# APIRest_Cuenta
Prueba Ingreso


1. Descargar
2. Descargar imagen docker sql (docker pull mcr.microsoft.com/mssql/server:2019-latest)
3. Instanciar imagen (localhost, 1460)
4. Crear database (create database gxcTest)
5. Correr scripts de creación (carpeta Sql Queries), en el siguiene orden:
          a. gxc_usuario.sql
          b. gxc_cuenta.sql
          c. gxc_cuenta_transaccion.sql
  Esto debido a la cardinalidad solicitada

6. Compilar poyecto (Test)
7. Importar el cliente RestAPI (Carpeta Postman)
8. Probar cada uno de los métodos:
          a. GXC USUARIO 
          b. GXC CUENTA
          c. GXC TRANSACCIONES (
              i. Generar Token en POST AUTENTICADOR
              ii. Ingresar Bearer Token en Auth de las otras operaciones
