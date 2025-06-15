Este proyecto es un videoclub clásico hecho como aplicación de consola visual usando .NET y Terminal.Gui. El objetivo es gestionar fácilmente las películas, los usuarios y los alquileres desde una terminal, con una experiencia sencilla y accesible.

¿Qué puede hacer la aplicación?

Permite añadir, editar, borrar, buscar y listar películas.

Puedes registrar nuevos usuarios, editar sus datos, borrarlos (si no tienen alquileres pendientes) y hay diferencia entre “admin” y usuarios normales.

Se pueden registrar alquileres y devoluciones de películas. Los usuarios pueden consultar todos sus alquileres.

Hay zona pública (para buscar y ver pelis) y una zona privada tras iniciar sesión con email y contraseña.

Los datos de películas, usuarios y alquileres se guardan y se leen de ficheros JSON, así que todo es persistente entre usos.

Se ha añadido un log de errores centralizado: si pasa algo raro o hay un fallo, el programa lo apunta en el fichero logs/log.txt. Así es fácil ver qué ha pasado y arreglarlo si hace falta.

Todo está dockerizado: el programa se puede arrancar en un contenedor Docker usando el puerto 27810 (como pide el profe). Además, los datos y los logs quedan accesibles desde fuera gracias a los volúmenes.

He añadido controles y mensajes de error claros para evitar bloqueos.

Un detalle importante: al contenerizar la aplicación, me encontré con que las aplicaciones de consola con GUI (como Terminal.Gui) sólo funcionan bien en modo interactivo, así que hay que lanzar el contenedor con -it y tener consola para verlo funcionar bien.

En definitiva, la aplicación cumple con lo que pedía el ejercicio: gestión total del videoclub, fácil de usar, robusta y ahora también preparada para funcionar en cualquier máquina gracias a Docker.
