# Concert Ticket Management System

This is a microservices-based web application for managing concerts, venues, and ticket bookings.

## Technologies Used

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- YARP (API Gateway)
- Swagger (API Documentation)

## Services

- **Event Service** – Manages concert events and schedules
- **Venue Service** – Handles venue information
- **Ticket Service** – Manages ticket bookings and availability
- **API Gateway** – Central access point using YARP for routing to all services

## Getting Started

1. Clone the repository
2. Configure the database connections in each service
3. Run all services and the API gateway
4. Access Swagger documentation at `/swagger` via the API Gateway

