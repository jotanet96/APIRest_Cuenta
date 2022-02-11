/*
author: Jose Araujo
*/

use gxctest

if object_id('dbo.cuenta_transaccion') is not null
	drop table cuenta_transaccion
go

create table cuenta_transaccion (
	id int identity(1,1) primary key,
	transaccion	varchar(254),
	num_cuenta varchar(16) 
);


