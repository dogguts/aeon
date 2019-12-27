rm src/Console1/TodoAppDb.sqlite
rm src/WebMvc/TodoAppDb.sqlite

rm -r src/Repository/Migrations 

dotnet ef migrations add Initial -p src/Repository/ -s src/Console1

dotnet ef database update -p src/Repository/ -s src/Console1
dotnet ef database update -p src/Repository/ -s src/WebMvc
