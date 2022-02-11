/*
author: Jose Araujo
*/


--create database gxcTest
use gxcTest

if object_id('dbo.usuario') is not null
	drop table usuario
go

create table usuario (
    cedula 		varchar(10) not null primary key,
	nombre		varchar(64) not null,
	apellido	varchar(64) not null,
	username	varchar(16) not null,
	password	varchar(500) not null,
	correo		varchar(64) not null,
	estado		bit default 0,
	sal			varchar(500)
);