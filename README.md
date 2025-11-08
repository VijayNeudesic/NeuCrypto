# NeuCrypto

[![Build Status](https://dev.azure.com/NeudesicNorthEast/Catholic%20Diocese%20of%20Pittsburgh%20-%20App%20Assessment/_apis/build/status%2FVijayNeudesic.NeuCrypto?branchName=main)](https://dev.azure.com/NeudesicNorthEast/Catholic%20Diocese%20of%20Pittsburgh%20-%20App%20Assessment/_build/latest?definitionId=97&branchName=main)

## Overview

NeuCrypto is a comprehensive .NET encryption library and toolset designed to provide enterprise-grade encryption and decryption capabilities for sensitive data. The library supports both individual text encryption/decryption and bulk database operations, making it ideal for protecting personally identifiable information (PII) in databases and applications.

## Main Functionality

### Core Encryption Capabilities

1. **Dual Encryption Algorithms**
   - **AES-256 Encryption**: High-performance symmetric encryption for bulk data operations
   - **RSA Encryption**: Asymmetric encryption using X.509 certificates for secure key management

2. **Certificate-Based Security**
   - Integrates with Windows Certificate Store
   - Uses X.509 certificates for RSA key management
   - Supports both LocalMachine and CurrentUser certificate stores

3. **Database Encryption**
   - Bulk encryption/decryption of database tables
   - Support for SQL Server and Microsoft Access databases
   - Batch processing with configurable batch sizes
   - Field-level encryption with WHERE clause filtering

### Key Components

#### 1. NeuCrypLib (Core Library)

The main cryptographic library containing:

- **Encryptor**: Main encryption orchestrator
  - Initializes RSA and AES encryption engines
  - Manages encryption/decryption operations
  - Handles logging and error management
  
- **AESEncType**: AES-256 symmetric encryption
  - 256-bit key encryption
  - 128-bit initialization vector (IV)
  - Base64 encoded output
  
- **RSAEncType**: RSA asymmetric encryption
  - Uses X.509 certificates
  - OAEP SHA-256 padding
  - Public key encryption, private key decryption
  
- **EncryptDB**: Database encryption engine
  - Executes bulk encrypt/decrypt operations on database tables
  - Supports ODBC connections for SQL Server and Access
  - Automatic field type detection and schema management
  
- **SSLCert**: Certificate management
  - Retrieves X.509 certificates from Windows Certificate Store
  - Validates certificate availability
  
- **AccessCode**: Time-based access code generator
  - Generates daily access codes for secure operations
  - SHA-256 hash-based code generation
  - Used to authorize bulk decryption operations

#### 2. NeuCrypto (COM-Visible Library)

- **CryptoProcess**: COM-visible wrapper class
  - Exposes encryption functionality to COM clients
  - Provides simplified API for external applications
  - Methods for string encryption/decryption and bulk database operations

#### 3. EncryptionTool (Windows Forms Application)

- GUI tool for database encryption operations
- Visual interface for configuring and executing bulk encryption/decryption
- User-friendly alternative to programmatic API usage

#### 4. GenerateCode (Console Utility)

- Command-line tool to generate daily access codes
- Essential for authorizing bulk decryption operations
- Provides time-based security mechanism

#### 5. TestCrypto and TestEncryption (Test Projects)

- Sample applications demonstrating library usage
- Performance testing for encryption algorithms
- Integration testing for database operations

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     Applications                         │
├─────────────────┬───────────────┬───────────────────────┤
│ EncryptionTool  │  TestCrypto   │  TestEncryption       │
│  (WinForms)     │  (Console)    │  (Console)            │
└────────┬────────┴───────┬───────┴───────────┬───────────┘
         │                │                   │
         └────────────────┼───────────────────┘
                          │
                ┌─────────▼─────────┐
                │   NeuCrypto       │
                │  (COM Wrapper)    │
                └─────────┬─────────┘
                          │
                ┌─────────▼─────────┐
                │   NeuCrypLib      │
                │  (Core Library)   │
                └─────────┬─────────┘
                          │
         ┌────────────────┼────────────────┐
         │                │                │
    ┌────▼────┐    ┌─────▼─────┐   ┌─────▼──────┐
    │  RSA    │    │    AES     │   │ EncryptDB  │
    │Encryption│    │ Encryption │   │  Engine    │
    └────┬────┘    └─────┬──────┘   └─────┬──────┘
         │               │                │
    ┌────▼─────┐         │          ┌─────▼──────┐
    │ SSLCert  │         │          │  Database  │
    │ (X.509)  │         │          │ (SQL/Access)│
    └──────────┘    ┌────▼─────┐    └────────────┘
                    │Registry   │
                    │Config     │
                    └───────────┘
```

## Key Features

### Encryption Data Marker
All encrypted data is prefixed with `_NDP_` header to:
- Identify encrypted vs. plain text data
- Prevent double encryption
- Enable safe decryption operations

### Registry-Based Configuration
The library reads encryption configuration from Windows Registry:
- Registry path: `HKLM\SOFTWARE\Neudesic\Neucrypto\`
- Stores certificate name and encrypted key material
- Provides centralized configuration management

### Security Features
1. **Certificate-based key management**: RSA keys derived from X.509 certificates
2. **Encrypted key storage**: Sensitive configuration stored encrypted in registry
3. **Access code protection**: Bulk decryption requires time-based access code
4. **Logging**: Comprehensive logging using Serilog framework
5. **Error handling**: Detailed error messages and status tracking

### Database Operations
- **Bulk Encryption**: Encrypt multiple fields in database tables
- **Bulk Decryption**: Decrypt encrypted fields (requires access code)
- **Filtered Operations**: Support for WHERE clause conditions
- **Performance Optimized**: Batch processing for large datasets
- **Schema Aware**: Automatic detection and handling of field types

## Usage Example

```csharp
// Initialize the crypto processor
CryptoProcess crypto = new CryptoProcess();
crypto.InitAll(@".\logs");

// Encrypt a string
string encrypted = crypto.EncryptString("Sensitive Data");

// Decrypt a string  
string decrypted = crypto.DecryptString(encrypted);

// Bulk encrypt database table
crypto.BulkEncryptDBTable(
    "ServerName",           // SQL Server name (empty for Access)
    "DatabasePath",         // Database path
    "TableName",           // Table to encrypt
    "Field1,Field2",       // Fields to encrypt
    "ID",                  // Key fields for WHERE clause
    ""                     // Filter operators
);

// Bulk decrypt (requires access code)
string accessCode = AccessCode.GenerateAccessCode();
crypto.BulkDecryptDBTable(
    "ServerName",
    "DatabasePath", 
    "TableName",
    "Field1,Field2",
    "ID",
    "",
    accessCode             // Daily access code
);
```

## System Requirements

- Windows operating system (certificate store integration)
- .NET Framework 4.7.2 or higher (for most projects)
- .NET 6.0 or higher (for TestCrypto)
- SQL Server or Microsoft Access (for database operations)
- X.509 certificate installed in Windows Certificate Store

## Security Considerations

- RSA encryption uses OAEP SHA-256 padding for enhanced security
- AES-256 provides strong symmetric encryption
- Certificate-based key management prevents key exposure
- Time-based access codes limit decryption authorization window
- All encryption operations are logged for audit purposes
- The `_NDP_` prefix prevents accidental double encryption

## Building the Solution

The solution is designed to be built on Windows with Visual Studio or MSBuild:

```bash
# Restore NuGet packages
nuget restore NeuCrypto.sln

# Build the solution
msbuild NeuCrypto.sln /p:Configuration=Release
```

Note: The solution targets .NET Framework and requires Windows-specific dependencies.

## Projects in Solution

| Project | Type | Purpose |
|---------|------|---------|
| NeuCrypLib | Class Library | Core encryption library |
| NeuCrypto | Class Library | COM-visible wrapper |
| EncryptionTool | Windows Forms | GUI application for encryption |
| GenerateCode | Console App | Access code generator |
| TestCrypto | Console App | Test application (newer) |
| TestEncryption | Console App | Test application (legacy) |
