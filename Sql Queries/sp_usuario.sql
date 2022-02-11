/*
author: Jose Araujo
*/

use gxctest


if object_id('dbo.sp_usuario') is not null
	drop procedure sp_usuario
go

create procedure sp_usuario (
	@i_operacion	char(1),
	@i_cedula 		varchar(10) = null,
	@i_nombre		varchar(64) = null,
	@i_apellido		varchar(64) = null,
	@i_username		varchar(16) = null,
	@i_password		varchar(64) = null,
	@i_correo		varchar(64) = null
)
as
declare
	@mensaje		nvarchar(max),
	@w_operacion	char(1),
	@w_cedula 		varchar(10),
	@w_nombre		varchar(64),
	@w_apellido		varchar(64),
	@w_username		varchar(16),
	@w_password		varchar(64),
	@w_correo		varchar(64)


if @i_operacion in('I', 'U') 
begin
	if @i_cedula is null or @i_cedula = ''
	begin
		set @mensaje = 'La cedula no puede ir vacía'
		raiserror (@mensaje,1,1) 
		return 1
	end
	
	if exists (select 1 from usuario where cedula = @i_cedula)
	begin
		set @mensaje = 'Ya existe un usuario con esta cédula: %s'
		raiserror (@mensaje,1,1, @i_cedula) 
		return 1
	end
		
	if exists (select 1 from usuario where username = @i_username)
	begin
		set @mensaje = 'Ya existe un usuario con este nombre de usuario: %s'
		raiserror (@mensaje,1,1, @i_username) 
		return 1
	end
		
	if exists (select 1 from usuario where correo = @i_correo)
	begin
		set @mensaje = 'Ya existe un usuario con este correo: %s'
		raiserror (@mensaje,1,1, @i_correo) 
		return 1
	end
	
	
end

if @i_operacion = 'I'
begin
	
	if @i_nombre is null or @i_nombre = ''
	begin
      	set @mensaje = 'El nombre no puede ir vacío'
		raiserror (@mensaje,1,1) 
		return 1
	end
	
	if @i_apellido is null or @i_apellido = ''
	begin
      	set @mensaje = 'El apellido no puede ir vacío'
		raiserror (@mensaje,1,1) 
		return 1
	end
	
	if @i_username is null or @i_username = ''
	begin
      	set @mensaje = 'El username no puede ir vacío'
		raiserror (@mensaje,1,1) 
		return 1
	end
	
	if @i_password is null or @i_password = ''
	begin
      	set @mensaje = 'La contraseña no puede ir vacío'
		raiserror (@mensaje,1,1) 
		return 1
	end
	
	if @i_correo is null or @i_correo = ''
	begin
      	set @mensaje = 'El correo no puede ir vacío'
		raiserror (@mensaje,1,1) 
		return 1
	end
	
	begin tran
		insert into usuario 
			(cedula,	nombre, 	apellido, 	username, 
				password, 	correo)
		values 
			(@i_cedula, 	@i_nombre, 	@i_apellido, 	@i_username, 
				@i_password, 	@i_correo)
			
		if @@error <> 0
	    begin
	      	set @mensaje = 'Error al insertar registro'
			raiserror (@mensaje,1,1) 
			return 1
	    end
    commit tran
end


if @i_operacion = 'U'
begin
	
	select
		@w_nombre	= nombre,
		@w_apellido = apellido,
		@w_username = username,
		@w_password = password,
		@w_correo	= correo
	from usuario
		where cedula = @i_cedula
		
	/*Validación campos a actualizar*/
	if @w_nombre  = @i_nombre
		select @w_nombre = null
	else
		select @w_nombre  = @i_nombre
		
	if @w_apellido  = @i_apellido
		select @w_apellido = null
	else
		select @w_apellido  = @i_apellido
		
	if @w_username  = @i_username
		select @w_username = null
	else
		select @w_username  = @i_username
		
	if @w_password  = @i_password
		select @w_password = null
	else
		select @w_password  = @i_password
		
	if @w_correo  = @i_correo
		select @w_correo = null
	else
		select @w_correo  = @i_correo
	
	begin tran
		update usuario set
			nombre		= isnull(@w_nombre, nombre),
			apellido	= isnull(@w_apellido, apellido),
			username	= isnull(@w_username, username),
			password	= isnull(@w_password, password),
			correo		= isnull(@w_correo, correo)
		where cedula = @i_cedula
		
			
		if @@error <> 0
	    begin
	      	set @mensaje = 'Error al actualizar registro'
			raiserror (@mensaje,1,1) 
			return 1
	    end
	commit tran
end

if @i_operacion = 'A'
begin
	
	select 
		cedula 		as 'Cedula',
		nombre 		as 'Nombres',
		apellido	as 'Apellidos',
		username	as 'Username',
		password	as 'Contrasena',
		correo		as 'Correo'
	from usuario
		where estado = 0
	
end


if @i_operacion = 'B'
begin
	
	select 
		cedula 		as 'Cedula',
		nombre 		as 'Nombres',
		apellido	as 'Apellidos',
		username	as 'Username',
		password	as 'Contrasena',
		correo		as 'Correo'
	from usuario
		where estado = 0
		and cedula = @i_cedula
	
end

if @i_operacion = 'D'
begin
	
	begin tran
		update usuario set
		estado = 1
		where cedula = @i_cedula
	commit tran
end
return 0
go