# Holiday Planner

## Overview

Holiday Planner is a web application designed to simplify holiday event planning for families and friends. The application helps users organize gatherings such as Christmas, Thanksgiving, birthdays, and other special events by providing collaborative planning tools, task management, participant tracking, and a gift exchange randomizer.

The goal of this project is to reduce the time spent coordinating logistics so users can focus more on creating meaningful experiences and spending quality time together.


## Purpose

Planning holiday events often involves coordinating schedules, assigning responsibilities, organizing participants, and managing event details. Holiday Planner centralizes these activities into a single platform where users can collaborate efficiently and keep all event information organized.


## Features

### User Authentication

* Create an account
* Secure user login
* User-specific event management

### Holiday Plan Management

* Create holiday events
* Edit event details
* Delete holiday plans
* View event summaries

### Event Details

* Event name
* Location
* Date and time
* Description
* Food planning
* Entertainment planning

### Collaboration

* Add collaborators to plans
* Manage participants
* Share planning responsibilities

### Task Management

* Create tasks
* Assign tasks
* Track task status
* Remove completed or unnecessary tasks

### Participant Management

* Add participant names
* Remove participants
* Maintain attendee lists

### Gift Exchange Randomizer

* Randomly assign gift exchange buddies
* Eliminate manual matching
* Create fair and unbiased pairings


## Target Audience

Holiday Planner is designed for:

* Families planning holiday gatherings
* Friends organizing celebrations
* Community groups
* Church groups
* Clubs and organizations
* Anyone coordinating events involving multiple participants



## Technologies Used

### Frontend

* HTML
* CSS
* JavaScript

### Backend

* Node.js
* Express.js

### Database

* PostgreSQL

### Authentication

* User Login and Registration System



## Database Design

### Users Table

| Field        | Description              |
| ------------ | ------------------------ |
| Username     | Unique username          |
| Display Name | User's display name      |
| Password     | Securely stored password |

### Plans Table

| Field         | Description         |
| ------------- | ------------------- |
| Event Name    | Name of event       |
| Location      | Event location      |
| Date          | Event date          |
| Description   | Event details       |
| Collaborators | Users collaborating |
| Participants  | Event attendees     |

### Tasks Table

| Field       | Description      |
| ----------- | ---------------- |
| User        | Assigned user    |
| Title       | Task title       |
| Description | Task description |
| Status      | Task progress    |



## Project Scope

### Included Features

* User account creation
* User login
* Event creation
* Event deletion
* Event collaboration
* Task creation and management
* Participant management
* Event summaries
* Gift exchange randomization

### Excluded Features

* User image uploads
* Collection of sensitive personal information
* Social media integration



## Installation

### Prerequisites

* Node.js
* PostgreSQL
* npm

### Clone Repository

```bash
git clone https://github.com/R-vanLangeveld/CSE325-Team-2.git
```

### Navigate to Project Folder

```bash
cd CSE325-Team-2
```

### Install Dependencies

```bash
npm install
```

### Configure Database

Create a PostgreSQL database and update the environment variables with your database credentials.

Example:

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=holidayplanner
DB_USER=postgres
DB_PASSWORD=yourpassword
```

### Start Application

```bash
npm start
```

Application will run locally on:

```text
http://localhost:5032
```



## Usage

1. Register a new account.
2. Log in to the application.
3. Create a holiday plan.
4. Add event details.
5. Invite collaborators.
6. Add participants.
7. Create and assign tasks.
8. Generate gift exchange assignments.
9. Review the event summary.
10. Enjoy your holiday gathering.



## Future Enhancements

* Email invitations
* Calendar integration
* SMS reminders
* Mobile application support
* Budget tracking
* Food contribution management
* Real-time notifications
* Multiple holiday templates



## Project Links

### GitHub Repository

https://github.com/R-vanLangeveld/CSE325-Team-2

### Trello Board

https://trello.com/b/3SBd0Nyl/project



## Authors

Holiday Planner Development Team

Developed as part of the CSE 325 Software Engineering Project.



## Reflection

This project provided valuable experience in full-stack web development, database integration, authentication systems, collaborative software design, and project management. It strengthened skills in planning, teamwork, debugging, database design, and creating solutions that address real-world organizational challenges.
