FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

# Listen ports
EXPOSE 80

# Install cultures (same approach as Alpine SDK image)
RUN apk add --no-cache icu-libs

# Disable the invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Copy files from other build jobs
COPY publish .

ENTRYPOINT ["dotnet", "Nager.AuthenticationService.WebApi.dll"]