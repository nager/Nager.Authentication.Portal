# Nager.AuthenticationService

This is an authentication microservice that is connected behind the `auth` path.
This generates JWT tokens which can then be used in other services. A Microsoft SQL Server is used as the database.

## Screenshots
![Demo 1](/doc/AuthenticationDemo1.png)
![Demo 2](/doc/AuthenticationDemo2.png)


## Docker Enviorment Variables

| VARIABLE                             | DESCRIPTION                                                              |
| ------------------------------------ | ------------------------------------------------------------------------ |
| CONNECTIONSTRINGS__DEFAULT           | Mssql Connection String                                                  |
| ASPNETCORE_ENVIRONMENT               | `Production` or `Development`                                            |
| AUTHENTICATION__TOKENS__ISSUER       | Identifies principal that issued the JWT.                                |
| AUTHENTICATION__TOKENS__AUDIENCE     | Identifies the recipients that the JWT is intended for.                  |
| AUTHENTICATION__TOKENS__SIGNINGKEY   | The cryptographic key that is used to generate the digital signature.    |
