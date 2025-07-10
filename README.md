PlayerVault ⚽
PlayerVault is a comprehensive and secure full-stack web application designed for managing football player data in a structured and user-friendly way. It streamlines player management for tournaments, clubs, and scouting platforms by providing a robust alternative to manual spreadsheets.

The application is built with a clean UI, a strong C# backend, and features role-based access control, making it ideal for real-world deployment.

👥 Authors
This project was a collaborative effort by:

Md Jawadur Rafid

Nabid Ahamed Noushad

Taif Tanjil Srabon

✨ Key Features
PlayerVault is built with a focus on security, functionality, and user experience.

Secure User Authentication: A complete registration and login system using session management and BCrypt-hashed passwords for security.

Role-Based Access Control: The application supports two main roles: Admin and Registered User, each with a dedicated and secure dashboard.

Users can submit, edit, and delete their own player profiles while tracking their approval status.

Admins have oversight to approve new users, manage all player submissions, and filter players by the user who submitted them.

CRUD Functionality: Full Create, Read, Update, and Delete (CRUD) operations for player profiles with validation-enhanced forms.

Dynamic Team Statistics: A dedicated page to view aggregate team stats, including average player age, market value, most common nationality, and more.

User-Friendly Interface: Features a toggleable dark mode, collapsible sections for pending players, and clear session feedback messages for a smooth user experience.

🛠️ Technical Stack
This project was built using the ASP.NET Core MVC framework, demonstrating a practical, full-stack development experience.

Frontend:

ASP.NET Core MVC: For structured, server-side rendered views.

Bootstrap 5: For responsive and consistent styling.

JavaScript: Used for dynamic features like the dark mode toggle.

Backend:

ASP.NET Core: For all controller logic, routing, and session management.

Entity Framework Core: Object-Relational Mapper (ORM) for managing the database.

Database:

Microsoft SQL Server: To store all user and player data.

Authentication:

Session-Based: To track user login status and protect routes.

BCrypt.Net: For secure password hashing.

🚀 Future Improvements
The architecture of PlayerVault is designed to be scalable and easily extended. Future enhancements could include:

Email-based account verification and notifications

A real-time analytics dashboard

Support for player profile images

Data import/export functionality

Advanced pagination and search optimization
