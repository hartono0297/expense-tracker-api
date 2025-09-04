# ---------- build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj first for restore caching
COPY ExpenseTracker.csproj ./
RUN dotnet restore ExpenseTracker.csproj

# copy the rest
COPY . .

# publish release to /app/publish
RUN dotnet publish ExpenseTracker.csproj -c Release -o /app/publish

# ---------- runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# copy published output from build stage
COPY --from=build /app/publish ./

# kestrel bind to container port 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_ENVIRONMENT=Production
EXPOSE 8080

# IMPORTANT: match your API dll name
ENTRYPOINT ["dotnet", "ExpenseTracker.dll"]
