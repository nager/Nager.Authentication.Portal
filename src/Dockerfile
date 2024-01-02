FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

# Avoid SQL Globalization Error
RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Listen ports
EXPOSE 8080

# Copy files from other build jobs
COPY publish .

ENTRYPOINT ["dotnet", "Nager.AuthenticationService.WebApi.dll"]