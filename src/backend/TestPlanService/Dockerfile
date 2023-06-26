FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY /bin/Release/net7.0/publish/ ./
ENTRYPOINT ["dotnet", "TestPlanService.dll"] 