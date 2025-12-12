FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# copy csproj and restore
COPY src/ ./
RUN dotnet restore

# copy everything and build
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
COPY --from=build /app/publish ./
EXPOSE 80
ENTRYPOINT ["dotnet", "ZavaStorefront.dll"]
