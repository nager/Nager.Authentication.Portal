# Nager.AuthenticationService

This microservice is responsible for handling authentication within your system.<br>
It operates independently and is deployed behind the `/auth` path, allowing seamless integration into your architecture.

## Features
- **JWT Token Generation**: Issues secure JSON Web Tokens (JWT) for use in other services.
- **Standalone Operation**: Runs independently to handle all authentication-related tasks.
- **Database Support**: Utilizes Microsoft SQL Server for secure storage and efficient data management.

## Screenshots
![Demo 1](/doc/AuthenticationDemo1.png)
![Demo 2](/doc/AuthenticationDemo2.png)

## Setup and Deployment

### Docker Enviorment Variables

| VARIABLE                             | DESCRIPTION                                                                             |
| ------------------------------------ | --------------------------------------------------------------------------------------- |
| ASPNETCORE_ENVIRONMENT               | `Production` or `Development`                                                           |
| CONNECTIONSTRINGS__DEFAULT           | Mssql Connection String                                                                 |
| AUTHENTICATION__TOKENS__ISSUER       | Specifies the entity or service that issued the JWT (e.g., your authentication server). |
| AUTHENTICATION__TOKENS__AUDIENCE     | Identifies the intended recipient(s) or service(s) for which the JWT is valid.          |
| AUTHENTICATION__TOKENS__SIGNINGKEY   | Defines the secret key or certificate used to sign and verify the integrity of the JWT. |
