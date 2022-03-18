# Eight360-Experiences-Project
Summer Internship at Hit Lab NZ

# Known Errors:

## ERROR 1

NullReferenceException: Object reference not set to an instance of an object
Player.TurretController.LateUpdate () (at Assets/MyAsset/MyScripts/Game-Related/BallTurret/TurretController.cs:185)

NullReferenceException: Object reference not set to an instance of an object
ScoreManager.LateUpdate () (at Assets/MyAsset/MyScripts/Game-Related/GameManagement/ScoreManager.cs:111)

###### Solution

This potentially occurs if there are no joystick connected during runtime. Try to plug in a joystick controller.
