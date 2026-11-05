FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY src/raxui-dashboard/raxui-dashboard.csproj src/raxui-dashboard/
RUN dotnet restore src/raxui-dashboard/raxui-dashboard.csproj
COPY src/raxui-dashboard/ src/raxui-dashboard/
RUN dotnet publish src/raxui-dashboard/raxui-dashboard.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
RUN mkdir -p /var/lib/raxui-dashboard/keys \
    && chown -R app:app /var/lib/raxui-dashboard
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "raxui-dashboard.dll"]
