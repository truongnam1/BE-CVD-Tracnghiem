dotnet ef dbcontext scaffold  "data source=42.96.33.23,20651;initial catalog=Tracnghiem;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;" Microsoft.EntityFrameworkCore.SqlServer -c DataContext  -o Models -f --no-build --use-database-names --json
$content = Get-Content -Path 'Models\DataContext.cs' -Encoding UTF8
$content = $content -replace "using System;", "using System;using Thinktecture;"
$content = $content -replace "modelBuilder.Entity<ActionDAO>", "modelBuilder.ConfigureTempTable<long>();modelBuilder.ConfigureTempTable<Guid>();modelBuilder.Entity<ActionDAO>"
$content | Set-Content -Path "Models\DataContext.cs"  -Encoding UTF8