FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 27810
#Ojo que estos son los datos que se guardan, es punto extra ;-)
VOLUME ["/app/data"]
ENTRYPOINT ["dotnet", "Videoclub.dll"]
