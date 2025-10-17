🎓 Online Quiz App (ASP.NET Core MVC + Identity)

An Online Quiz Management System built with ASP.NET Core MVC, Entity Framework Core, and Identity.
It supports Admin Panel for quiz management and Student Panel for taking quizzes and tracking results.

✨ Features
👨‍🏫 Admin Panel

Manage quizzes (create, edit, delete, activate/deactivate).

Add questions and multiple choice options.

Mark correct answers.

View and manage all quiz attempts.

👨‍🎓 Student Panel

Browse available quizzes with title, description, and total questions.

Take quizzes with multiple choice questions.

Automatic scoring with percentage calculation.

View quiz results (score, correct/incorrect answers).

User-friendly dashboard with responsive Bootstrap cards.

🛠️ Tech Stack

Backend: ASP.NET Core MVC 7, C#

Database: SQL Server (via EF Core)

Authentication & Roles: ASP.NET Core Identity

Admin → Manage quizzes & questions

Student → Take quizzes and view results

Frontend: Razor Views + Bootstrap 5 + Bootstrap Icons

🚀 Getting Started
1. Clone the repository
git clone https://github.com/<your-username>/quiz-app.git
cd quiz-app

2. Update Database
dotnet ef database update

3. Run the Application
dotnet run


Open 👉 https://localhost:5073

🔑 Default Roles & Users

When the app starts, it seeds default roles:

Admin

Student

You can manually assign users to roles via the database or seed method.

📸 Screenshots

Admin Dashboard → Manage quizzes & questions

Student Dashboard → View quizzes in card layout & take quizzes

Quiz Results → Score with correct/incorrect answers

📌 Future Improvements

Add quiz time limits ⏱️

Export quiz results to PDF/Excel 📄

Leaderboard & analytics 📊

Email notifications ✉️

👨‍💻 Author

Developed by Md. Arman Ahmed ✨
