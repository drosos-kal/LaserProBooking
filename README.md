# LaserPro - Client & Booking Management System

LaserPro is a full-stack web application designed to manage clients and bookings for a laser treatment clinic. The project uses a DotVVM C# framework for the backend, MongoDB for database management, and features a user-friendly frontend built with JavaScript and Bootstrap. The application is deployed on an EC2 instance using NGINX, with regular scheduled backups for data integrity.

## Features

- **Client Management**: Add, update, and view clients' information.
- **Booking Management**: Manage appointments and booking schedules for treatments.
- **Data Backup**: Automated backups of client data to ensure safety and recovery.
- **Frontend**: Built with smooth user experience in mind.
- **EC2 Deployment**: Hosted on AWS EC2 with NGINX for high availability and performance.
- **User Authentication**: Cookie-based authentication for secure login functionality.
- **Admin Page**: A simple admin page to add new users.

## Technology Stack

### Backend
- **DotVVM (C#)**: For building the server-side logic and managing API calls.
- **MongoDB**: NoSQL database for managing and storing client and booking data.
- **Scheduled Backups**: Automated processes for data backup to ensure data persistence.

### Frontend
- **JavaScript**: For dynamic client-side functionality.
- **Bootstrap**: A responsive design framework for ensuring mobile-friendly user interfaces.

### Deployment
- **AWS EC2**: The application is hosted on an Amazon EC2 instance, ensuring scalability and availability.
- **Nginx**: Used as a reverse proxy server to handle HTTP requests and improve performance.

## Backup Strategy

- The project includes a scheduled task that backs up MongoDB data weekly to an external storage solution.
- Regular backups ensure data integrity and recovery.

## Future Enhancements

- **Analytics Dashboard**: Display key metrics like total bookings, popular treatment times, etc.
- **Expanded Admin Page**: Include features for managing users.

