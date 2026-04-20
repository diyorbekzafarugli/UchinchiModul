# 1. Loyihani build qilish uchun .NET 10 SDK ni olamiz
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# 2. Hamma .csproj fayllarini o'z papkasiga nusxalaymiz
COPY ["src/StudentCoursePlatform.Domain/StudentCoursePlatform.Domain.csproj", "src/StudentCoursePlatform.Domain/"]
COPY ["src/StudentCoursePlatform.Application/StudentCoursePlatform.Application.csproj", "src/StudentCoursePlatform.Application/"]
COPY ["src/StudentCoursePlatform.Infrastructure/StudentCoursePlatform.Infrastructure.csproj", "src/StudentCoursePlatform.Infrastructure/"]
COPY ["src/StudentCoursePlatform.Api/StudentCoursePlatform.Api.csproj", "src/StudentCoursePlatform.Api/"]

# 3. Kutubxonalarni yuklaymiz (Restore)
RUN dotnet restore "src/StudentCoursePlatform.Api/StudentCoursePlatform.Api.csproj"

# 4. Endi qolgan barcha kodlarni ham kopiya qilamiz
COPY . .

# 5. Loyihani kompyilatsiya qilish (Publish)
WORKDIR "/app/src/StudentCoursePlatform.Api"
RUN dotnet publish "StudentCoursePlatform.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. Faqat ishlashi uchun yengil Runtime'ni yuklab olamiz
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 7. Asosiy dll faylni ishga tushiramiz
ENTRYPOINT ["dotnet", "StudentCoursePlatform.Api.dll"]