# Snipe-IT Playwright Automation

## Overview

This project automates key asset management workflows in the Snipe-IT demo application using C#, Microsoft Playwright, and NUnit.

The primary objective is to validate the asset creation workflow, verify the created asset's details, and confirm that the relevant asset history records are displayed correctly.

The project follows the **Page Object Model (POM)** design pattern to separate page interactions from test scenarios and assertions.

## Technologies Used

- C#
- .NET 10
- Microsoft Playwright
- NUnit
- Chromium
- Page Object Model (POM)

## Project Structure

```text
SnipeIt.Automation/
├── SnipeIt.Automation.Tests/
│   ├── Infrastructure/
│   ├── Models/
│   ├── Pages/
│   │   ├── AssetPage.cs
│   │   └── LoginPage.cs
│   ├── Tests/
│   │   ├── AssetTest.cs
│   │   └── LoginTest.cs
│   ├── BrowserSmokeTests.cs
│   ├── SnipeIt.Automation.Tests.csproj
│   └── README.md
└── SnipeIt.Automation.slnx
```

**Note:** The structure above reflects the current project layout. The `bin/` and `obj/` directories are generated during builds and should normally be excluded from Git using `.gitignore`.

## Application Under Test

- **Application:** Snipe-IT Demo
- **URL:** https://demo.snipeitapp.com/

## Automated Test Scenarios

### 1. Browser Smoke Test

- Launch the browser and navigate to the Snipe-IT application.
- Verify that the browser navigates to the expected application URL.
- Verify that the page title is not empty.

### 2. Login Test

- Navigate to the Snipe-IT application.
- Enter valid login credentials.
- Submit the login form.
- Verify that login succeeds and the application navigates to the expected page.

### 3. Asset Creation, Details, and History Verification

- Log in to the application.
- Navigate to the Assets page.
- Open the Create Asset form.
- Select a company.
- Enter a serial number.
- Select the Macbook Pro 13 model.
- Set the asset status to **Ready to Deploy**.
- Assign the asset to a user.
- Enter notes.
- Capture the generated Asset Tag.
- Save the asset.
- Search for the newly created asset using its generated Asset Tag.
- Open the asset details page.
- Verify the asset information displayed on the Details page.
- Open the History tab and verify the relevant history records.

#### Details Page Verification

The test verifies the following information, where applicable:

- Asset Tag
- Model
- Serial Number
- Notes
- Category
- Status
- Assigned User
- Checkout Information

The assigned user's name is captured during asset creation and compared with the username displayed on the Details page.

#### History Verification

**Create New Record**

Verify the relevant history information:

- Asset Tag
- Model
- Created By user
- Quantity

**Checkout Record**

Verify the relevant history information:

- Asset Tag
- Model
- Created By user
- Assigned target user
- Quantity

The History verification also checks that the assigned target user's name matches the user selected during asset creation.

## Prerequisites

Before running the tests, ensure the following are available:

- .NET 10 SDK
- Microsoft Playwright NuGet package and the project's required dependencies
- Chromium browser installed through Playwright
- Access to the Snipe-IT demo application
- Valid demo login credentials

## Setup Instructions

### 1. Clone or Download the Repository

Clone the repository or download the project files to your local machine.

### 2. Open the Project

Open a terminal in the directory containing `SnipeIt.Automation.slnx`, or navigate to the test project directory.

### 3. Restore NuGet Packages

Run the following command from the solution directory:

```bash
dotnet restore
```

### 4. Build the Project

Run:

```bash
dotnet build
```

### 5. Install the Playwright Chromium Browser

After building the test project, run the following command in PowerShell from the test project directory:

```powershell
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

If the Playwright script is generated in a different output directory, adjust the path accordingly.

## Running the Tests

Run all tests from the solution directory:

```bash
dotnet test
```

Run only the browser smoke test:

```bash
dotnet test --filter "FullyQualifiedName~BrowserSmokeTests"
```

Run only the login test:

```bash
dotnet test --filter "FullyQualifiedName~LoginTests"
```

Run only the asset creation, Details, and History verification test:

```bash
dotnet test --filter "FullyQualifiedName~CreateAsset_SearchAndVerifyDetailsAndHistory"
```

To display detailed test execution output, use:

```bash
dotnet test --filter "FullyQualifiedName~CreateAsset_SearchAndVerifyDetailsAndHistory" --logger "console;verbosity=normal"
```

The browser is configured to run in headed mode when `Headless = false`.

## Design Approach

This project follows the **Page Object Model (POM)** design pattern.

- **Pages:** Contain page-specific locators and browser interactions.
- **Tests:** Define test scenarios, assertions, and verification of expected results.
- **Test Data:** Uses generated serial numbers and notes to reduce conflicts between test runs.
- **Asset Identification:** Captures the generated Asset Tag and reuses it to locate the newly created asset during verification.

This separation helps keep the automation code readable, reusable, and maintainable.

## Test Data

- Unique serial numbers and notes are generated during asset creation to reduce conflicts between test runs.
- The generated Asset Tag is captured before the asset is saved.
- The captured Asset Tag is used to search for the created asset.
- The selected user's name is captured after selecting the user and reused in the Details and History assertions.

## Notes

- This project uses the public Snipe-IT demo application for automation testing. The demo application's availability, configuration, and data may change at any time.
- The tests may be affected by changes to the application's user interface, demo data, or configuration.
- The `LoginPage.cs` page object currently contains the demo username (`admin`) and password (`password`) provided for accessing the public demo environment. These are demo credentials, not personal account credentials.
- The demo credentials are hardcoded for simplicity in this learning and assignment project. For production projects or environments using private accounts, credentials should be supplied through environment variables or a secure secret-management solution instead of being stored in source code.
- Use only authorized test accounts and environments.
- Never commit personal passwords, access tokens, private credentials, or other sensitive information to the repository.
- The `.gitignore` file excludes generated build directories, such as `bin/` and `obj/`, and test results from version control.
- Update this README when additional test scenarios, dependencies, or setup requirements are introduced.

## Author

Melani Madhushani
