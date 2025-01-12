# Nager.AuthenticationService

This is an authentication microservice that is connected behind the `auth` path.
This generates JWT tokens which can then be used in other services. A Microsoft SQL Server is used as the database.

## Screenshots
![Demo 1](/doc/AuthenticationDemo1.png)
![Demo 2](/doc/AuthenticationDemo2.png)


## Docker Enviorment Variables

| VARIABLE                             | DESCRIPTION                                                                             |
| ------------------------------------ | --------------------------------------------------------------------------------------- |
| ASPNETCORE_ENVIRONMENT               | `Production` or `Development`                                                           |
| CONNECTIONSTRINGS__DEFAULT           | Mssql Connection String                                                                 |
| AUTHENTICATION__TOKENS__ISSUER       | Specifies the entity or service that issued the JWT (e.g., your authentication server). |
| AUTHENTICATION__TOKENS__AUDIENCE     | Identifies the intended recipient(s) or service(s) for which the JWT is valid.          |
| AUTHENTICATION__TOKENS__SIGNINGKEY   | Defines the secret key or certificate used to sign and verify the integrity of the JWT. |
