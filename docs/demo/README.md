# Application Demo
A visual walkthrough of the Recipe Manager application.
The application is currently demonstrated through recorded local sessions rather than a live deployment.

## Authentication
### 1. Unknown email → Register
When an email that is not associated with an account is submitted on the Login screen, the application shows a registration prompt.

https://github.com/user-attachments/assets/337861a4-4709-448f-8ba2-50bf0f19896d


### 2. Registration
A new user can create an account with their first name, last name, email, and optional phone number.
The registration flow also includes requesting and resending the verification code.

https://github.com/user-attachments/assets/9e27e0d5-10ca-48d1-a578-5d48881901a7

https://github.com/user-attachments/assets/11885770-8a61-45b3-9b2a-51b8e13d2cd6


### 3. Login verification → Explore
An existing user requests a verification code, enters it, and is authenticated into the application.

<img width="473" height="57" alt="03-request-code-verify-explore" src="https://github.com/user-attachments/assets/68a7e839-fc43-47dd-896b-7088d2470e8f" />

https://github.com/user-attachments/assets/279d3e23-9c36-4786-bd1d-bbabf05b58bf

> In the local development environment, the verification code is printed in the API console rather than sent by email.

## Recipes
### 4. Create and manage recipes
Users can browse the shared recipe collection, open recipe details, and create and manage their own recipes.

Recipe creation supports dynamically added ingredient rows.

https://github.com/user-attachments/assets/1dbc6dda-d6d8-4d7a-a07b-05e6f7117cb9

Recipe actions follow ownership rules. Users can manage their own recipes, while shared recipes remain available for viewing.

https://github.com/user-attachments/assets/eba5ab85-23ca-4901-b311-e27d782d824f

https://github.com/user-attachments/assets/656e5ed7-beb2-4be4-ad7a-210adaa7548b

## Favorites
### 5. Add and remove favorites
Users can add recipes to Favorites and remove them again.

https://github.com/user-attachments/assets/d326a158-2ec8-4d43-b6ee-b3ea1faf1c46

## Profile
### 6. Profile management

Users can update their first name, last name, and phone number.

https://github.com/user-attachments/assets/79d79985-dfd2-4e8e-a27e-3cc875998791

Users can also delete their account.

https://github.com/user-attachments/assets/d9123ccb-2ec1-4faf-b4f1-4601b9adbead


## Logout
### 7. Logout
Signing out returns the user to the Login screen. Protected areas require an authenticated session.

https://github.com/user-attachments/assets/f69ea112-ae3c-43b7-8bb0-990c929039db


## Administration
The application includes a dedicated administration area for users with the Administrator role.

### 8. Admin access
Administrators use the same passwordless authentication flow as regular users.
After authentication, the ADMIN section becomes available in the sidebar with access to Users, Recipes, and Lookups.

<img width="455" height="49" alt="08-admin-login" src="https://github.com/user-attachments/assets/fc1fb070-e3a5-4768-81bd-72968ae4cf29" />

https://github.com/user-attachments/assets/ffae1f5c-50a2-4283-998f-7a792125455a


### 9. User management
Administrators can view users, change user roles, and delete users.
The application also prevents removing or deleting the last administrator.

https://github.com/user-attachments/assets/8b3da980-6872-4659-90e5-25747bb8a8f7


### 10. Recipe management
Administrators can manage recipes across the application, including searching by title or author and deleting recipes.

https://github.com/user-attachments/assets/fe6e35e2-708c-4682-a435-ddf6d19058d6

https://github.com/user-attachments/assets/61f94843-cb1d-46c5-9056-694b441c8ebf


### 11. Lookup management
Administrators can manage Categories, Cuisines, and Ingredients through the lookup section.
Unused lookup values can be deleted, while values that are still referenced by recipes cannot be removed.

https://github.com/user-attachments/assets/3276948f-4d22-413f-896b-6976b74be392

## Feature coverage

| Area           | Functionality                                          |
| -------------- | ------------------------------------------------------ |
| Authentication | Passwordless login, registration, verification, logout |
| Recipes        | Browse, view, create, edit, delete                     |
| Favorites      | Add and remove favorites                               |
| Profile        | Update and delete account                              |
| Authorization  | User / Administrator roles and ownership rules         |
| Admin          | User, recipe, and lookup management                    |
| Validation     | Protected routes and business-rule validation          |

The application is currently available as a local development demo rather than a live deployment.
