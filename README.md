# University Management System

A .NET Core web application for managing university resources, rooms, courses, and user roles.

## Features

- User Authentication and Authorization
- Course Management
- Room Management and Booking
- Cleaning Request System
- TA/JTA Management
- Student Quota System
- Supply Request Management
- Reporting System

## Prerequisites

- .NET 6.0 or later
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code with C# extensions

## Setup Instructions

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run the following commands:
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

4. Navigate to `https://localhost:5001` or `http://localhost:5000`

## Project Structure

- `Models/` - Contains all data models and database context
- `Pages/` - Razor pages for the web interface
- `wwwroot/` - Static files (CSS, JS, images)
- `Properties/` - Application configuration

## Security Notes

- Update the connection string in `appsettings.json` before deployment
- Enable HTTPS in production
- Implement proper password hashing
- Configure proper session management

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 