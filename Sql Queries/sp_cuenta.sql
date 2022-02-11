/*
author: Jose Araujo
*/

use gxctest


if object_id('dbo.sp_cuenta') is not null
	drop procedure sp_cuenta
go

create procedure sp_cuenta (
	@i_operacion	char(1),
	@i_cedula 		varchar(10) = null,
	@i_num_cuenta	varchar(16) = null,
	@i_valor		decimal 	= null,
	@i_cedula_b		varchar(10)	= null,
	@i_num_cuenta_b varchar(16) = null
)G
as
declare
	@mensaje		nvarchar(max),
	@w_saldo		decimal 		= null,
	@w_transaccion	varchar(254)	= null


if @i_operacion in('X') 
begin
	if not exists (select 1 from usuario where cedula = @i_cedula)
	begin
		set @mensaje = 'No existe el usuario con cédula: %s'
		raiserror (@mensaje,1,1, @i_cedula) 
		return 1
	end
	
	if not exists (select 1 from cuenta where num_cuenta = @i_num_cuenta and cedula = @i_cedula)
	begin
		set @mensaje = 'El usuario no tiene asignado este numero de cuenta: %s'
		raiserror (@mensaje,1,1, @i_num_cuenta) 
		return 1
	end
end



if @i_operacion in('T') 
begin
	if not exists (select 1 from usuario where cedula = @i_cedula)
	begin
		set @mensaje = 'El usuario con cédula: %s'
		raiserror (@mensaje,1,1, @i_cedula) 
		return 1
	end
	
	if not exists (select 1 from cuenta where num_cuenta = @i_num_cuenta)
	begin
		set @mensaje = 'La cuenta número: %s no existe.'
		raiserror (@mensaje,1,1, @i_num_cuenta) 
		return 1
	end
	
	if not exists (select 1 from cuenta where num_cuenta = @i_num_cuenta and cedula = @i_cedula)
	begin
		set @mensaje = 'El usuario no tiene asignado el número de cuenta.'
		raiserror (@mensaje,1,1) 
		return 1
	end
	
	
	if not exists (select 1 from usuario where cedula = @i_cedula_b)
	begin
		set @mensaje = 'El usuario con cédula: %s'
		raiserror (@mensaje,1,1, @i_cedula_b) 
		return 1
	end
	
	if not exists (select 1 from cuenta where num_cuenta = @i_num_cuenta_b)
	begin
		set @mensaje = 'La cuenta número: %s no existe.'
		raiserror (@mensaje,1,1, @i_num_cuenta) 
		return 1
	end
	
	if not exists (select 1 from cuenta where num_cuenta = @i_num_cuenta_b and cedula = @i_cedula_b)
	begin
		set @mensaje = 'El usuario no tiene asignado el número de cuenta.'
		raiserror (@mensaje,1,1) 
	end
end


/*CREACIÓN DE CUENTA*/
if @i_operacion = 'I'
begin

	if not exists (select 1 from usuario where cedula = @i_cedula)
	begin
		set @mensaje = 'No existe el usuario con cédula: %s'
		raiserror (@mensaje,1,1, @i_cedula) 
		return 1
	end

	if exists (select 1 from cuenta where num_cuenta = @i_num_cuenta)
	begin
		set @mensaje = 'La cuenta número: %s ya existe'
		raiserror (@mensaje,1,1, @i_num_cuenta) 
		return 1
	end

	begin tran
		if @i_valor is not null and @i_valor > 0
		begin
			insert into cuenta 
				(cedula,		num_cuenta,		saldo)
			values 
				(@i_cedula, 	@i_num_cuenta, 	@i_valor)
			
			if @@error <> 0
		    begin
		      	set @mensaje = 'Error al insertar registro'
				raiserror (@mensaje,1,1) 
				return 1
		    end
				
				
			select @w_transaccion = 'Se acreditó el valor de: $' +  cast(@i_valor as varchar(18))
	    
			insert into cuenta_transaccion 
				(num_cuenta,	transaccion)
			values
				(@i_num_cuenta,		@w_transaccion)
				
			if @@error <> 0
		    begin
		      	set @mensaje = 'Error al actualizar registro.'
				raiserror (@mensaje,1,1) 
				return 1
		    end
		
		end
		else
		begin
			if @i_valor is null or @i_valor < 0
			begin
				insert into cuenta 
					(cedula,		num_cuenta)
				values 
					(@i_cedula, 	@i_num_cuenta)
			end
			
			if @@error <> 0
		    begin
		      	set @mensaje = 'Error al actualizar registro.'
				raiserror (@mensaje,1,1) 
				return 1
		    end
		end		
    commit tran
end

/*DEPÓSITO MONETARIO*/
if @i_operacion = 'X'
begin
	
	if @i_valor <= 0
	begin
      	set @mensaje = 'Error al valor a ingresar debe de ser mayor a 0'
		raiserror (@mensaje,1,1) 
		return 1
    end
		
	select @w_saldo = saldo from cuenta where num_cuenta = @i_num_cuenta
	set @w_saldo = @w_saldo + @i_valor
	
	begin tran
		update cuenta set
			saldo = @w_saldo
		where cedula 		= @i_cedula
			and num_cuenta  = @i_num_cuenta
		
			
		if @@error <> 0
	    begin
	      	set @mensaje = 'Error al actualizar registro'
			raiserror (@mensaje,1,1) 
			return 1
	    end
	    
	    select @w_transaccion = 'Se acreditó el valor de: $' +  cast(@i_valor as varchar(18))
	    
		insert into cuenta_transaccion 
			(num_cuenta,	transaccion)
		values
			(@i_num_cuenta,		@w_transaccion)
			
		if @@error <> 0
	    begin
	      	set @mensaje = 'Error al actualizar registro.'
			raiserror (@mensaje,1,1) 
			return 1
	    end
	commit tran
end

/*TRANSFERENCIA*/
if @i_operacion = 'T'
begin
	--DE:   Cta A  --> Cta B
	
	--Saldo cuenta A
	select @w_saldo = saldo from cuenta where num_cuenta = @i_num_cuenta and cedula = @i_cedula
	
	if @i_valor > @w_saldo
	begin
	  	set @mensaje = 'No cuenta con saldo disponible.'
		raiserror (@mensaje,1,1) 
		return 1
	end


	
	begin tran
		--DESCUENTA CTA A
		
	 	select @w_saldo = @w_saldo - @i_valor
	
		update cuenta set
			saldo = @w_saldo
		where cedula 		= @i_cedula
			and num_cuenta  = @i_num_cuenta
			
		if @@error <> 0
	    begin
	      	set @mensaje = 'Error al actualizar registro.'
			raiserror (@mensaje,1,1) 
			return 1
	    end
	    
	    select @w_transaccion = 'Se debitó el valor de: $' +  cast(@i_valor as varchar(18))
	    
		insert into cuenta_transaccion 
			(num_cuenta,	transaccion)
		values
			(@i_num_cuenta,		@w_transaccion)
			
		if @@error <> 0
	    begin
	      	set @mensaje = 'Error al actualizar registro'
			raiserror (@mensaje,1,1) 
			return 1
	    end
	    
	    
	    
		--ACREDITA CTA B
		--Saldo cuenta B
		select @w_saldo = saldo from cuenta where num_cuenta = @i_num_cuenta_b and cedula = @i_cedula_b
	 	select @w_saldo = @w_saldo + @i_valor
		
		update cuenta set
			saldo = @w_saldo
		where cedula 		= @i_cedula_b
			and num_cuenta  = @i_num_cuenta_b
		
		if @@error <> 0
	    begin
	      	set @mensaje = 'Error al actualizar registro'
			raiserror (@mensaje,1,1) 
			return 1
	    end
	    
	    select @w_transaccion = 'Se acreditó el valor de: $' +  cast(@i_valor as varchar(18))
	    
		insert into cuenta_transaccion 
			(num_cuenta,	transaccion)
		values
			(@i_num_cuenta_b,		@w_transaccion)
			
		if @@error <> 0
	    begin
	      	set @mensaje = 'Error al actualizar registro'
			raiserror (@mensaje,1,1) 
			return 1
	    end
	commit tran
end



/*Consulta histórico cuenta*/
if @i_operacion = 'A'
begin
	select transaccion from cuenta_transaccion ct inner join cuenta c on c.num_cuenta = ct.num_cuenta where ct.num_cuenta = @i_num_cuenta and c.estado = 0
end



/*Consulta cuentas*/
if @i_operacion = 'B'
begin
	select 
		cedula,
		num_cuenta,
		saldo
	from cuenta where estado = 0
end

/*Consulta cuentas by cedula*/
if @i_operacion = 'C'
begin
	select 
		num_cuenta,
		saldo
	from cuenta where estado = 0 and cedula = @i_cedula
end

/*Eliminar cuenta*/
if @i_operacion = 'D'
begin
	
	begin tran
		update cuenta set
		estado = 1
		where cedula = @i_cedula
		and num_cuenta = @i_num_cuenta
	commit tran
end

return 0
go