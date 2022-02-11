/*
author: Jose Araujo
*/

use gxctest

if object_id('dbo.cuenta') is not null
	drop table cuenta
go

create table cuenta (
	num_cuenta	varchar(16) not null primary key,
	saldo		decimal default 0.0,
	estado		bit default 0,
	
	cedula varchar(10) foreign key references usuario(cedula)
);


