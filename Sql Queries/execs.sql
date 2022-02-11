-- *** SqlDbx Personal Edition ***
-- !!! Not licensed for commercial use beyound 90 days evaluation period !!!
-- For version limitations please check http://www.sqldbx.com/personal_edition.htm
-- Number of queries executed: 105, number of rows retrieved: 0

use GXCTest

exec sp_usuario @i_operacion = 'I',
			    @i_cedula = '1717171717',
			    @i_nombre = 'Carlos',
			    @i_apellido = 'Peres',
			    @i_username = 'cape',
			    @i_password = '123patito',
			    @i_correo = 'cape@gmail.com'


exec sp_usuario @i_operacion = 'A'

exec sp_usuario @i_operacion = 'B',
				@i_cedula = '1717171717'


/********************************************************/
/********************************************************/
/********************************************************/
/********************************************************/
/********************************************************/
/********************************************************/
exec sp_cuenta  @i_operacion = 'I', --INSERTA
			    @i_cedula = '0802502948',
			    @i_num_cuenta = '1111111',
			    @i_valor = 200

exec sp_cuenta  @i_operacion = 'T', --TRANSFERENCIA
			    @i_cedula = '0802502948',
			    @i_num_cuenta = '1111111',
			    @i_valor = 200,
			    @i_cedula_b = '0802502948',
			    @i_num_cuenta_b = '1111112'
			    
exec sp_cuenta  @i_operacion = 'X', --DEPOSITO
			    @i_cedula = '0802502948',
			    @i_num_cuenta = '1111111',
			    @i_valor = 200

exec sp_cuenta @i_operacion = 'A', --SELECT BY ID
			   @i_num_cuenta = '111111'
exec sp_cuenta @i_operacion = 'A',
			   @i_num_cuenta = '123456'
			   
			   
exec sp_cuenta @i_operacion = 'B' --SELECT ALL

exec sp_cuenta @i_operacion = 'D', --ELIMINA CUENTA
			    @i_cedula = '0802502948',
			    @i_num_cuenta = '123456'
			
			