rem Run as administrator
rem Change the username and/or port as needed

netsh http add urlacl url=http://+:8011/ user=bzamecnik
