# Minutes of Meeting Management System

A comprehensive .NET-based web application for managing organizational meetings, built with ASP.NET Core MVC architecture.

---

## 📋 Project Overview

This is a **Minutes of Meeting (MOM) Management System** designed to streamline the process of organizing, tracking, and documenting organizational meetings. It provides a centralized platform for managing meeting details, attendees, venues, and meeting types.

### Key Information
- **Framework**: ASP.NET Core (net10.0)
- **Architecture**: MVC (Model-View-Controller)
- **Language Composition**:
  - HTML: 49.2%
  - C#: 34.7%
  - CSS: 12.6%
  - JavaScript: 3.3%
  - Java: 0.2%
- **Repository Created**: February 14, 2026
- **Last Updated**: March 25, 2026

---

## 🏗️ Project Architecture

The application follows the **MVC pattern** with the following structure:

```
MOM/
├── Controllers/          # Business logic and request handlers
├── Models/              # Data models and view models
├── Views/               # Razor templates for UI rendering
├── wwwroot/             # Static assets (CSS, JavaScript, images)
└── appsettings.json     # Configuration settings
```

---

## 🎮 Core Controllers

### 1. **HomeController**
- **Purpose**: Dashboard and main landing page
- **Key Features**:
  - Displays meeting statistics (total, upcoming, completed, cancelled)
  - Generates charts for meetings by type and department
  - Requires admin authentication
  - Session-based user tracking

### 2. **AdminAccountController**
- **Purpose**: User authentication and account management
- **Key Actions**:
  - `Register`: New admin account creation
  - `Login`: User authentication with email/password
  - `Logout`: Session termination
- **Features**:
  - Email validation and duplicate checking
  - Session management
  - Error handling for authentication failures

### 3. **DepartmentController**
- **Purpose**: Manage organizational departments
- **Key Actions**:
  - `DepartmentList`: View all departments with search
  - `DepartmentAddEdit`: Create/update departments
  - `DepartmentDelete`: Remove departments
- **Features**:
  - Search functionality
  - Soft delete handling with relationship validation
  - Timestamp tracking (Created/Modified)

### 4. **MeetingTypeController**
- **Purpose**: Manage different types of meetings
- **Key Actions**:
  - `MeetingTypeList`: View all meeting types
  - `MeetingTypeAddEdit`: Create/update meeting types
  - `GetMeetingTypes`: Retrieve with optional search
- **Fields**: Meeting Type Name, Remarks, Created/Modified dates

### 5. **MeetingVenueController**
- **Purpose**: Manage meeting locations/venues
- **Key Actions**:
  - `MeetingVenueList`: View all venues
  - `MeetingVenueAddEdit`: Add/edit venues
  - `MeetingVenueDelete`: Remove venues
- **Features**: Search functionality, timestamp tracking

### 6. **StaffController**
- **Purpose**: Manage staff members and employees
- **Key Actions**:
  - `StaffList`: View staff with department filtering
  - `StaffAddEdit`: Create/update staff records
  - `StaffDelete`: Remove staff members
- **Features**:
  - Department-based filtering
  - Contact information (email, mobile)
  - Staff statistics population

### 7. **AttendanceController**
- **Purpose**: Track meeting attendance records
- **Key Actions**:
  - `AttendanceList`: View attendance with date filtering
  - Date range filtering (start date to end date)
- **Features**:
  - Meeting member presence tracking
  - Remarks for attendance notes
  - Related data joining (meeting, staff, department)

### 8. **MeetingController**
- **Purpose**: Core meeting management
- **Key Actions**:
  - `MeetingList`: View all meetings with search
  - `MeetingDetail`: View detailed meeting information
  - Create/edit meetings
- **Features**:
  - Document attachment handling
  - Meeting cancellation tracking
  - Relationship with type, department, and venue

---

## 📊 Data Models

### Key Models

#### **AdminUserModel**
```csharp
- AdminID
- FullName (required)
- Email (required, validated)
- Password (required)
- ConfirmPassword (validation comparison)
```

#### **Department**
```csharp
- DepartmentID
- DepartmentName
- Created (DateTime)
- Modified (DateTime)
```

#### **MeetingType**
```csharp
- MeetingTypeID
- MeetingTypeName
- Remarks
- Created (DateTime)
- Modified (DateTime)
```

#### **MeetingVenue**
```csharp
- MeetingVenueID
- MeetingVenueName
- Created (DateTime)
- Modified (DateTime)
```

#### **Staff**
```csharp
- StaffID
- DepartmentID (Foreign Key)
- StaffName
- Mobile
- Email
- Remarks
- Department (Navigation)
- Created (DateTime)
- Modified (DateTime)
```

#### **Meeting**
```csharp
- MeetingID
- MeetingDate
- MeetingTypeID (Foreign Key)
- DepartmentID (Foreign Key)
- MeetingVenueID (Foreign Key)
- MeetingDescription
- DocumentPath
- IsCancelled (Boolean)
- Relations: MeetingType, Department, MeetingVenue
```

#### **MeetingMember**
```csharp
- MeetingMemberID
- MeetingID (Foreign Key)
- StaffID (Foreign Key)
- IsPresent (Boolean)
- Remarks
- Meeting (Navigation)
- Staff (Navigation)
- Created (DateTime)
- Modified (DateTime)
```

#### **DashboardViewModel**
```csharp
- TotalMeetings (int)
- UpcomingMeetings (int)
- CompletedMeetings (int)
- CancelledMeetings (int)
- MeetingsByTypeLabels (List<string>)
- MeetingsByTypeSeries (List<int>)
- MeetingsByDepartmentLabels (List<string>)
- MeetingsByDepartmentSeries (List<int>)
```

---

## 🗄️ Database Access

### Technology Stack
- **Primary**: SQL Server via `Microsoft.Data.SqlClient`
- **ORM Support**: Entity Framework Core (EF Core 10.0.2)
- **Connection String**: Configured in `appsettings.json` as "DefaultConnection"

### Data Access Pattern
- **Stored Procedures** (Recommended approach):
  - `PR_AdminUser_Login`
  - `PR_AdminUser_Register`
  - `PR_AdminUser_SelectByEmail`
  - `PR_MOM_Department_SelectAll`
  - `PR_MOM_Department_SelectByPK`
  - `PR_MOM_Department_DeleteByPK`
  - `PR_MOM_MeetingType_SelectAll`
  - `PR_MOM_MeetingType_SelectByPK`
  - `PR_MOM_MeetingVenue_SelectAll`
  - `PR_MOM_MeetingVenue_SelectByPK`
  - `PR_MOM_Meetings_SelectAll`
  - `PR_MOM_Staff_SelectAll`
  - And more...

- **Direct SQL Queries**: Used in some controllers (e.g., `AttendanceController`)

### Connection Pattern
```csharp
SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
SqlCommand cmd = new SqlCommand();
cmd.Connection = con;
cmd.CommandText = "StoredProcedureName";
cmd.CommandType = CommandType.StoredProcedure;
```

---

## 🔐 Authentication & Authorization

### Session-Based Authentication
- **Session Keys**:
  - `AdminID`: Unique administrator identifier
  - `FullName`: Administrator's full name
  - `Email`: Administrator's email address
  - `City`: Administrator's city (optional)

### Protection Mechanism
- Protected routes redirect unauthenticated users to `AdminAccount/Login`
- Session validation check:
  ```csharp
  if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
  {
      return RedirectToAction("Login", "AdminAccount");
  }
  ```

---

## 🎨 Frontend Technologies

### Styling & UI
- **CSS**: 12.6% of codebase
- **HTML**: 49.2% of codebase
- **JavaScript**: 3.3% of codebase

### Libraries Used
- **Chart.js**: For data visualization (dashboard charts)
- **Bootstrap**: Likely used for responsive design (vendor folder indicates third-party libs)

### UI Features
- Dashboard with charts and statistics
- Responsive form layouts for CRUD operations
- Search and filter capabilities
- Date range pickers for attendance tracking

---

## 📦 Dependencies

### NuGet Packages
```xml
- Microsoft.Data.SqlClient (v6.1.1): SQL Server connectivity
- Microsoft.EntityFrameworkCore.SqlServer (v10.0.2): ORM support
- Microsoft.EntityFrameworkCore.Tools (v10.0.2): EF Core utilities
- System.Data.SqlClient (v4.9.1): Legacy SQL support
```

### .NET Framework
- **.NET 10.0**: Latest stable framework
- **Nullable Reference Types**: Enabled for type safety
- **Implicit Usings**: Enabled for reduced namespace clutter

---

## 🚀 Key Features

### 1. **Dashboard**
- Real-time meeting statistics
- Visual charts for meeting distribution
- Meeting status tracking (upcoming, completed, cancelled)
- Department-wise meeting breakdown

### 2. **Meeting Management**
- Create and schedule meetings
- Attach meeting documents
- Cancel meetings if needed
- Categorize by type and venue
- Assign to departments

### 3. **Attendee Management**
- Register staff members
- Track attendance presence
- Filter by department
- Add attendance remarks

### 4. **Administrative Functions**
- User registration and login
- Department management
- Meeting type classification
- Venue management

### 5. **Reporting & Analytics**
- Meeting statistics by type
- Meeting distribution by department
- Attendance tracking with date filters
- Historical data retention

---

## 🔄 Data Flow

```
User Login
    ↓
Dashboard (View Statistics & Charts)
    ↓
Manage Meetings
├── Define Meeting Types
├── Create Departments
├── Register Venues
└── Create Staff
    ↓
Schedule & Execute Meetings
    ↓
Track Attendance
    ↓
Generate Reports
```

---

## 📝 Common Workflows

### Create a Meeting
1. Admin logs in
2. Navigate to Meeting Management
3. Fill meeting details (date, type, venue, department)
4. Add meeting description
5. Upload meeting document
6. Save meeting

### Track Attendance
1. Go to Attendance section
2. Filter by date range (optional)
3. View all meeting members
4. Mark presence/absence
5. Add remarks if needed

### Manage Departments
1. Navigate to Department List
2. Search for existing departments
3. Add new or edit existing departments
4. Delete if no related records exist

---

## ⚙️ Configuration

### Connection String
Located in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<server>;Database=<db>;User Id=<user>;Password=<password>;"
  }
}
```

### Session Configuration
- Session-based authentication with server-side session storage
- Session keys managed through `HttpContext.Session`

---

## 🛠️ Development Notes

### Code Quality Observations
- **Dependency Injection**: Controllers use constructor injection for `IConfiguration`
- **Error Handling**: Try-catch blocks for database operations
- **Validation**: Model-level validation with data annotations
- **Null Checks**: Defensive programming with null coalescing

### Database Best Practices
- Stored procedures for data access (recommended pattern)
- Parameterized queries to prevent SQL injection
- Transaction handling where needed

### UI/UX Patterns
- Search functionality on list pages
- Add/Edit forms with validation feedback
- Date pickers for date-based inputs
- Department filtering on staff list

---

## 📚 Additional Resources

- **Framework Documentation**: [ASP.NET Core MVC](https://learn.microsoft.com/aspnet/core)
- **Database Driver**: [Microsoft.Data.SqlClient](https://learn.microsoft.com/sql/connect/ado-net/introduction-microsoft-data-sqlclient)
- **ORM**: [Entity Framework Core](https://learn.microsoft.com/ef/core)

---

## 🎯 Summary

The **Minutes of Meeting Management System** is a well-structured MVC application designed for corporate meeting management. It provides comprehensive features for scheduling, tracking, and reporting on organizational meetings with a focus on department-based organization and attendance tracking. The system uses SQL Server as its backend with a mix of stored procedures and direct queries for data access, alongside a responsive web interface built with HTML, CSS, and JavaScript.

