# Financial Analysis Assistant - PHASE 1: Project Setup & Core Infrastructure

## Overview
This phase establishes the foundation of the application including solution structure, domain models, and basic project configuration.

**Estimated Time:** 2-3 hours  
**Prerequisites:** Visual Studio 2022, .NET 9 SDK, SQL Server or SQLite

---

## Step 1.1: Create Solution and Projects

### Copilot Prompt:
```
Create a new .NET 9 solution named "FinancialAnalysisAssistant" with the following structure:

1. Create a blank solution
2. Add these projects:
   - FinancialAnalysisAssistant.Web (Blazor Server App template)
   - FinancialAnalysisAssistant.Core (Class Library)
   - FinancialAnalysisAssistant.Infrastructure (Class Library)
   - FinancialAnalysisAssistant.Services (Class Library)

3. Add project references:
   - Web project references: Core, Infrastructure, Services
   - Services project references: Core, Infrastructure
   - Infrastructure project references: Core

4. Remove default Class1.cs files from all class library projects
```

### Manual Steps:
1. Open Visual Studio 2022
2. Create New Project → Blank Solution
3. Right-click solution → Add → New Project (repeat for each project above)
4. Right-click each project → Add → Project Reference (add references as listed)

---

## Step 1.2: Install NuGet Packages

### Copilot Prompt for Web Project:
```
In FinancialAnalysisAssistant.Web project, install the following NuGet packages:

dotnet add package MudBlazor --version 7.8.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0

Update the project file to target .NET 9.0 if not already set
```

### Copilot Prompt for Infrastructure Project:
```
In FinancialAnalysisAssistant.Infrastructure project, install:

dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
```

### Copilot Prompt for Services Project:
```
In FinancialAnalysisAssistant.Services project, install:

dotnet add package ClosedXML --version 0.102.3
dotnet add package iTextSharp.LGPLv2.Core --version 3.4.22
dotnet add package Newtonsoft.Json --version 13.0.3
dotnet add package OllamaSharp --version 3.0.0
dotnet add package CsvHelper --version 33.0.1
dotnet add package Polly --version 8.4.0
```

---

## Step 1.3: Create Folder Structure

### Copilot Prompt:
```
Create the following folder structure in each project:

FinancialAnalysisAssistant.Core/
├── Entities/
├── Enums/
├── DTOs/
└── Interfaces/

FinancialAnalysisAssistant.Infrastructure/
├── Data/
│   └── Migrations/
├── Repositories/
└── FileStorage/

FinancialAnalysisAssistant.Services/
├── AI/
├── Financial/
└── Reports/

FinancialAnalysisAssistant.Web/
├── Pages/
├── Components/
└── Shared/

Add empty .gitkeep files in each folder to preserve structure in git
```

---

## Step 1.4: Create Domain Entities

### Copilot Prompt for FinancialDocument Entity:
```
In FinancialAnalysisAssistant.Core/Entities/, create FinancialDocument.cs with the following requirements:

Create a class named FinancialDocument with these properties:
- Id: int (primary key, auto-generated)
- FileName: string (required, max length 255)
- FileType: string (required, max length 50)
- UploadDate: DateTime (required, defaults to UTC now)
- FileSize: long (required, represents bytes)
- FilePath: string (required, max length 500)
- Status: string (required, max length 50)
- CreatedBy: string (nullable, max length 100)

Add navigation properties:
- FinancialDataRecords: ICollection<FinancialData> (one-to-many)
- AIAnalyses: ICollection<AIAnalysis> (one-to-many)

Use data annotations for validation:
- [Key] for Id
- [Required] for required fields
- [MaxLength(n)] for string lengths
- Initialize collections in constructor to avoid null reference

Include XML documentation comments for each property
```

### Copilot Prompt for FinancialData Entity:
```
In FinancialAnalysisAssistant.Core/Entities/, create FinancialData.cs:

Create a class named FinancialData with these properties:
- Id: int (primary key)
- DocumentId: int (foreign key to FinancialDocument, required)
- AccountName: string (required, max length 200)
- AccountCode: string (nullable, max length 50)
- Period: string (required, max length 20) - format like "2024-Q1" or "2024-01"
- Amount: decimal (required, precision 18, scale 2)
- Currency: string (required, max length 3, default "USD")
- Category: string (required, max length 50)
- SubCategory: string (nullable, max length 100)
- DateRecorded: DateTime (required)

Add navigation property:
- Document: FinancialDocument (many-to-one)

Use appropriate data annotations
Add [Column(TypeName = "decimal(18,2)")] for Amount property
Include XML documentation
```

### Copilot Prompt for AIAnalysis Entity:
```
In FinancialAnalysisAssistant.Core/Entities/, create AIAnalysis.cs:

Create a class named AIAnalysis with properties:
- Id: int (primary key)
- DocumentId: int (foreign key, required)
- AnalysisType: string (required, max length 50)
- Prompt: string (required, stores the prompt sent to AI)
- Response: string (required, stores AI response, no max length)
- ModelUsed: string (nullable, max length 50)
- ExecutionTime: int (required, milliseconds)
- CreatedDate: DateTime (required, defaults to UTC now)
- Rating: int? (nullable, range 1-5 for user feedback)

Navigation property:
- Document: FinancialDocument

Use data annotations including [Range(1, 5)] for Rating
Add XML documentation comments
```

### Copilot Prompt for Report Entity:
```
In FinancialAnalysisAssistant.Core/Entities/, create Report.cs:

Create a class named Report with properties:
- Id: int (primary key)
- Title: string (required, max length 200)
- Description: string (nullable, max length 1000)
- ReportType: string (required, max length 50)
- GeneratedDate: DateTime (required, defaults to UTC now)
- Content: string (required, stores JSON or HTML content)
- Parameters: string (nullable, stores JSON parameters used to generate report)

No navigation properties needed
Use appropriate data annotations
Include XML documentation
```

---

## Step 1.5: Create Enums

### Copilot Prompt for All Enums:
```
In FinancialAnalysisAssistant.Core/Enums/, create the following enum files:

1. DocumentStatus.cs:
public enum DocumentStatus
{
    Uploaded = 0,
    Processing = 1,
    Analyzed = 2,
    Error = 3,
    Archived = 4
}

2. AnalysisType.cs:
public enum AnalysisType
{
    Summary = 0,
    TrendAnalysis = 1,
    AnomalyDetection = 2,
    Comparison = 3,
    Forecasting = 4,
    RatioAnalysis = 5,
    Custom = 6
}

3. FinancialCategory.cs:
public enum FinancialCategory
{
    Revenue = 0,
    Expense = 1,
    Asset = 2,
    Liability = 3,
    Equity = 4
}

4. FileType.cs:
public enum FileType
{
    Unknown = 0,
    Excel = 1,
    CSV = 2,
    PDF = 3
}

Add XML documentation comments explaining each enum value
```

---

## Step 1.6: Create DTOs

### Copilot Prompt for AnalysisRequestDto:
```
In FinancialAnalysisAssistant.Core/DTOs/, create AnalysisRequestDto.cs:

Create a class for requesting AI analysis with properties:
- DocumentId: int (required)
- AnalysisType: AnalysisType enum (required)
- CustomPrompt: string (optional, allows user to provide custom instructions)
- IncludeCharts: bool (default false)
- StartDate: DateTime? (nullable, for filtering data by date range)
- EndDate: DateTime? (nullable)
- ComparisonPeriod: string (nullable, for comparison analysis)

Add data annotations for validation:
- [Required] where appropriate
- [Range] for positive integers
- Custom validation: EndDate must be after StartDate if both provided

Include parameterless constructor and constructor with required parameters
```

### Copilot Prompt for AnalysisResponseDto:
```
In FinancialAnalysisAssistant.Core/DTOs/, create AnalysisResponseDto.cs:

Create a class for AI analysis results with properties:
- AnalysisId: int
- DocumentId: int
- AnalysisType: AnalysisType enum
- Summary: string (brief overview)
- DetailedAnalysis: string (full analysis text)
- KeyFindings: List<string> (bullet points of main findings)
- Recommendations: List<string> (actionable recommendations)
- ChartData: Dictionary<string, object> (nullable, JSON-serializable chart data)
- ExecutionTime: int (milliseconds)
- GeneratedDate: DateTime
- ModelUsed: string

Initialize collections in constructor
Add method: ToJson() that serializes to JSON string
```

### Copilot Prompt for UploadResultDto:
```
In FinancialAnalysisAssistant.Core/DTOs/, create UploadResultDto.cs:

Create a class for document upload results with properties:
- Success: bool
- DocumentId: int? (nullable, only set if successful)
- FileName: string
- RecordsImported: int (number of financial data records extracted)
- ErrorMessages: List<string> (list of any errors encountered)
- Warnings: List<string> (list of warnings, e.g., missing fields)
- ProcessingTime: int (milliseconds)

Add helper methods:
- AddError(string message)
- AddWarning(string message)
- HasErrors: bool property (computed)
- HasWarnings: bool property (computed)

Initialize collections in constructor
```

### Copilot Prompt for FinancialDataDto:
```
In FinancialAnalysisAssistant.Core/DTOs/, create FinancialDataDto.cs:

Create a DTO that mirrors FinancialData entity but for data transfer:
- Id: int
- DocumentId: int
- AccountName: string
- AccountCode: string
- Period: string
- Amount: decimal
- Currency: string
- Category: string
- SubCategory: string
- DateRecorded: DateTime
- DocumentFileName: string (additional, from joined data)

Add method: ToEntity() that converts DTO to FinancialData entity
Add static method: FromEntity(FinancialData entity) that converts entity to DTO
```

---

## Step 1.7: Verification Checklist

Before moving to Phase 2, verify:

- [ ] All 4 projects created and building successfully
- [ ] All NuGet packages installed without errors
- [ ] Folder structure created in all projects
- [ ] 4 entity classes created with proper data annotations
- [ ] 4 enum files created
- [ ] 4 DTO classes created
- [ ] No compilation errors in solution
- [ ] Project references configured correctly

**Build the entire solution** (Ctrl+Shift+B) to ensure everything compiles.

---

## Next Phase Preview

**Phase 2** will cover:
- Creating DbContext
- Implementing Repository Pattern
- Database migrations
- Connection string configuration

---

## Troubleshooting

### Common Issues:

**Issue:** NuGet package conflicts  
**Solution:** Ensure all projects target .NET 9.0, clean solution and rebuild

**Issue:** OllamaSharp package not found  
**Solution:** Add NuGet source or use alternative: `dotnet add package OllamaSharp --source https://api.nuget.org/v3/index.json`

**Issue:** Circular dependency warnings  
**Solution:** Verify project references match the architecture (Core has no dependencies, Infrastructure references Core only)

---

## Files Created This Phase

```
✓ FinancialAnalysisAssistant.Core/Entities/FinancialDocument.cs
✓ FinancialAnalysisAssistant.Core/Entities/FinancialData.cs
✓ FinancialAnalysisAssistant.Core/Entities/AIAnalysis.cs
✓ FinancialAnalysisAssistant.Core/Entities/Report.cs
✓ FinancialAnalysisAssistant.Core/Enums/DocumentStatus.cs
✓ FinancialAnalysisAssistant.Core/Enums/AnalysisType.cs
✓ FinancialAnalysisAssistant.Core/Enums/FinancialCategory.cs
✓ FinancialAnalysisAssistant.Core/Enums/FileType.cs
✓ FinancialAnalysisAssistant.Core/DTOs/AnalysisRequestDto.cs
✓ FinancialAnalysisAssistant.Core/DTOs/AnalysisResponseDto.cs
✓ FinancialAnalysisAssistant.Core/DTOs/UploadResultDto.cs
✓ FinancialAnalysisAssistant.Core/DTOs/FinancialDataDto.cs
```

**Total Files:** 12  
**Lines of Code:** ~600-800 (estimated)