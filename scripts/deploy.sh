#!/usr/bin/env bash
set -e

# Construye la imagen (etiqueta tuusuario/videoclub:1.0)
docker build -t tuusuario/videoclub:1.0 .

# Sube al registro (Docker Hub u otro)
docker push tuusuario/videoclub:1.0

# Descarga y ejecuta localmente
docker pull tuusuario/videoclub:1.0
docker rm -f videoclub 2>/dev/null || true
docker run -d \
  --name videoclub \
  -p 27810:27810 \
  -v "$(pwd)/data:/app/data" \
  tuusuario/videoclub:1.0

echo " Videoclub corriendo en http://localhost:27810"
