# CapLed / PartFinder

CapLed est une solution de gestion ERP orientée pièces industrielles, stocks, commandes, lead management et suivi commercial.  
Le projet combine une API backend, une application desktop WPF, un front web moderne et une couche analytique pour soutenir les opérations de gestion et la décision.

Le nom “PartFinder” est également utilisé pour désigner la partie web/catalogue et les flux de recherche/consultation de stock et produits.

## Vue d’ensemble

CapLed a pour objectif de centraliser la gestion des activités commerciales et logistiques d’une entreprise :
- gestion des articles / produits / familles / catégories
- gestion des stocks et mouvements de stock
- suivi des alertes de stock
- gestion des leads / demandes clients
- gestion des commandes et livraisons
- authentification et autorisation des utilisateurs
- interface desktop pour les utilisateurs internes
- interface web pour la consultation / catalogue / usages métiers
- architecture backend API sécurisée avec JWT
- persistance MySQL
- déploiement via Docker et pipelines CI/CD

---

## Architecture

Le projet est organisé selon une architecture modulaire et multi-couches :

- API ASP.NET Core : point d’entrée principal pour les services métiers
- Core : modèles métier, services, interfaces, DTOs, logique applicative
- Infrastructure : persistance, repositories, services externes, MySQL / EF Core
- Desktop WPF : interface utilisateur pour les opérations internes
- Web React/Vite : interface front pour catalogue et gestion métier
- Analytics : composants d’analyse de données et KPI

```mermaid
flowchart LR
    User[Utilisateur] --> Desktop[CapLed.Desktop WPF]
    User --> Web[CapLed Catalog / Web]
    Desktop --> API[CapLed.API ASP.NET Core]
    Web --> API
    API --> Core[CapLed.Core]
    API --> Infra[CapLed.Infrastructure]
    Infra --> DB[(MySQL)]
    API --> Analytics[Analytics / Reporting]
