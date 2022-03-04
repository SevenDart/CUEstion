FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /source

EXPOSE 4000
EXPOSE 80

# copy and build app and libraries
COPY CUEstion.BLL/ CUEstion.BLL/
COPY CUEstion.WEB/ CUEstion.WEB/
COPY CUEstion.DAL/ CUEstion.DAL/
COPY CUEstion.sln .
RUN dotnet build -c release

FROM build AS publish
RUN dotnet publish -c release --no-build -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CUEstion.WEB.dll"]