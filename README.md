📘 BookZone API
BookZone est une API RESTful développée avec ASP.NET Core pour gérer une plateforme de vente de livres en ligne. Elle permet l’authentification, la gestion des utilisateurs, 
le catalogue de livres, les commandes, le panier, ainsi qu’un système de notification par email.

🌐 Fonctionnalités principales
🔐 Authentification & Autorisation avec JWT

👤 Gestion des rôles (Admin, Client)

📚 CRUD pour les livres

🛒 Gestion du panier

🧾 Création et historique des commandes

🚚 Suivi du statut des commandes (EnAttente, EnCours, Expediee, Livree, Annulee)

📧 Envoi d’e-mails via Mailtrap (confirmation de commande, etc.)

🛡️ Swagger UI avec support JWT pour les tests

🛠️ Technologies utilisées
ASP.NET Core Web API (.NET 8)

Entity Framework Core

SQL Server

Identity (Auth & Rôles)

JWT (Authentification)

MailTrap (pour test d'e-mails)

Swagger (documentation et test API)
