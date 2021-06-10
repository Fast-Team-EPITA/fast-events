#  Prérequis
- Instance SQL serveur
- ASP.NET Core 5.0 Runtime : https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-5.0.5-windows-hosting-bundle-installer

#  SQL Serveur
#############

- Ouvrir  SSMS
- Cliquer  sur  *Connecter*  ->  *Moteur  de base de données...*
- Selectionner  le nom de votre serveur SQL dans la liste
- Cliquer  *Connexion*
---
- Ouvrir un invité de commande dans l'emplacement du  README
- lancer la commande en remplaçant  myServer  avec le nom de votre instance SQL  Server  
  ``sqlcmd  -S  [myServer]  -i .\script.sql  ``

#  IIS

- Ouvrir le gestionnaire de services Internet  (IIS)
- Dans  pool  d'application,  cliquer  *Ajouter  un pool d'application*
- Entrer "FastEventsPool" dans le champ  *Nom*
 ---
- Sélectionner  *FastEventsPool*  ->  *Parametres  Avancés...*
- Sélectionner  *Modèle  de  processus*  ->  *Identité*  ->  *Compte  personnalisé*
- Entrer votre nom d'utilisateur  windows  et votre mot de passe
- Cliquer  *OK*
--- 
- Click-droit sur  *Sites*  ->  *Ajouter  un nouveau site  Web...*
- Entrer "FastEvents" dans le champ  *Nom  du  site*
- Sélectionner  "FastEventsPool" avec le bouton  *Selectionner*
- Choisir le chemin ou sera  deployer  le site
- Cliquer  *OK*

#  Déploiement

- Ouvrir  FastEvents.sln  avec  Visual  Studio en tant qu'administrateur
- Click-droit sur le projet "FastEvents"  ->  *Publier...*
- Sélectionner  *Services  connectés*  ->  *Configurer*  (a  droite de  *Base  de données de serveur SQL)
- Sélectionner  Base de données locale SQL  Servr  Express  (local)
- Entrer  DbFastEvents  dans le champ  *Nom  de la  chaine  de  connexion*
- Cliquer  *...*  dans le champ  *Valeur  de la  chaine  de  connexion*
- Entrer le nom de votre serveur SQL dans le champ  *Nom  du  serveur*
- Sélectionner  "FastEvents" dans le champ  *Sélectionner  ou entrer un nom de base de  données*
- Cliquer  *Avancées*  et copier  la  string de connexion
- Cliquer  *OK* -> *OK* ->  *Suivant* ->  *Terminer*  ->  *Fermer*
 ---
- Ouvrir le fichier  FastEvents/DataAccess/EfModels/FastEventContext.cs
- Remplacer  ligne  29 "CONNECTION_STRING" par ce que vous avez copiez a l'étape  d'au-dessus
- Sauvegarder le fichier
 ---
- Retourner sur l'onglet  *Publier*
- Sélectionner  *Publier*  ->  *+  Nouvelle*  ->  *Serveur  web  (IIS)*  ->  *Web  Deploy*
- Compléter  le formulaire avec les paramètres suivants

| Champ| Valeur|
|--|--|
| Serveur | localhost |
| Nom du site | FastEvents |
| URL de destination | http://localhost:80 (remplacer 80 par le port choisi dans IIS)|

- Cliquer  *Terminer* -> *Publier*