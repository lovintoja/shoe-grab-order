FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ./../shoe-grab-order/*.csproj ./shoe-grab-order/
COPY ./../shoe-grab-common/ ./shoe-grab-common/
COPY ./../shoe-grab-order/ ./shoe-grab-order/

RUN dotnet restore ./shoe-grab-order/*.csproj
RUN dotnet publish ./shoe-grab-order/*.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /out .
EXPOSE 10011
EXPOSE 10012
ENV DB_CONNECTION_STRING="Host=your-db-vm-ip;Port=5432;Database=your-db-name;Username=your-username;Password=your-password;"
ENTRYPOINT ["dotnet", "shoe-grab-order.dll"]
