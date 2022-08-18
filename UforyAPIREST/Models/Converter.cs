using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models.Responses;

namespace UforyAPIREST.Models
{
    public class Converter
    {
        public static UsuariosResponse UsuarioAUsuarioResponse(Usuarios usuario)
        {
            return new UsuariosResponse()
            {
                Email = usuario.Email,
                Suscripcion = usuario.Suscripcion
            };
        }

        public static ProyectosResponse ProyectoAProyectoResponse(Proyectos proyecto)
        {
            return new ProyectosResponse()
            {
                IdProyecto = proyecto.IdProyecto,
                Nombre = proyecto.Nombre,
                UltimaMod = proyecto.UltimaMod
            };
        }

        public static List<ProyectosResponse> LProyectoALProyectoResponse(List<Proyectos> proyectos)
        {
            List<ProyectosResponse> proyectoResponse = new List<ProyectosResponse>();
            foreach (var item in proyectos)
            {
                proyectoResponse.Add(ProyectoAProyectoResponse(item));
            }
            return proyectoResponse;
        }

        public static TareasResponse TareasATareasResponse(Tareas tarea)
        {
            return new TareasResponse()
            {
                IdTarea = tarea.IdTarea,
                Contenido = tarea.Contenido,
                Completada = tarea.Completada,
                Posicion = tarea.Posicion
            };
        }

        public static List<TareasResponse> LTareaALTareaResponse(List<Tareas> tareas)
        {
            List<TareasResponse> tareasResponse = new List<TareasResponse>();
            foreach (var item in tareas)
            {
                tareasResponse.Add(TareasATareasResponse(item));
            }
            return tareasResponse;
        }

        public static NotasResponse NotaANotaResponse(Notas nota)
        {
            return new NotasResponse()
            {
                IdNota = nota.IdNota,
                Nombre = nota.Nombre,
                UltimaMod = nota.UltimaMod,
                Contenido = nota.Contenido
            };
        }
        public static List<NotasResponse> LNotaALNotaResponse(List<Notas> notas)
        {
            List<NotasResponse> notaResponse = new List<NotasResponse>();
            foreach (var item in notas)
            {
                notaResponse.Add(NotaANotaResponse(item));
            }
            return notaResponse;
        }

        public static ArchivosResponse ArchivoAArchivoResponse(Archivos archivo)
        {
            return new ArchivosResponse()
            {
                IdArchivo = archivo.IdArchivo,
                NombreCloud = archivo.NombreCloud,
                Tipo = archivo.Tipo,
                Nombre = archivo.Nombre,
                UltimaMod = archivo.UltimaMod,
                Peso = archivo.Peso
            };
        }
        public static List<ArchivosResponse> LArchivoALArchivoResponse(List<Archivos> archivos)
        {
            List<ArchivosResponse> archivoResponse = new List<ArchivosResponse>();
            foreach (var item in archivos)
            {
                archivoResponse.Add(ArchivoAArchivoResponse(item));
            }
            return archivoResponse;
        }

        public static ComentariosResponse ComentarioAComentarioResponse(Comentarios comentario)
        {
            return new ComentariosResponse()
            {
                IdComentario = comentario.IdComentario,
                Contenido = comentario.Contenido,
                PosH = comentario.PosH,
                PosV = comentario.PosV,
                TiempoInicio = comentario.TiempoInicio,
                TiempoFin = comentario.TiempoFin
            };
        }
        public static List<ComentariosResponse> LComentarioALComentarioResponse(List<Comentarios> comentarios)
        {
            List<ComentariosResponse> comentarioResponse = new List<ComentariosResponse>();
            foreach (var item in comentarios)
            {
                comentarioResponse.Add(ComentarioAComentarioResponse(item));
            }
            return comentarioResponse;
        }



        public static DibujosImagenResponse DibujoADibujosImagenResponse(Dibujos dibujo)
        {
            return new DibujosImagenResponse()
            {
                IdDibujo = dibujo.IdDibujo,
                NombreCloud = dibujo.NombreCloud,
                Identificador = dibujo.Identificador
            };
        }

        public static List<DibujosImagenResponse> LDibujoALDibujosImagenResponse(List<Dibujos> dibujos)
        {
            List<DibujosImagenResponse> dibujosResponse = new List<DibujosImagenResponse>();
            foreach (var item in dibujos)
            {
                dibujosResponse.Add(DibujoADibujosImagenResponse(item));
            }
            return dibujosResponse;
        }

        public static DibujosAudioResponse DibujoADibujosAudioResponse(Dibujos dibujo)
        {
            return new DibujosAudioResponse()
            {
                IdDibujo = dibujo.IdDibujo,
                NombreCloud = dibujo.NombreCloud,
                Identificador = dibujo.Identificador,
                TiempoInicio = (int)dibujo.TiempoInicio,
                TiempoFin = (int)dibujo.TiempoFin
            };
        }

        public static List<DibujosAudioResponse> LDibujoALDibujosAudioResponse(List<Dibujos> dibujos)
        {
            List<DibujosAudioResponse> dibujosResponse = new List<DibujosAudioResponse>();
            foreach (var item in dibujos)
            {
                dibujosResponse.Add(DibujoADibujosAudioResponse(item));
            }
            return dibujosResponse;
        }

        public static DibujosVideoResponse DibujoADibujosVideoResponse(Dibujos dibujo)
        {
            return new DibujosVideoResponse()
            {
                IdDibujo = dibujo.IdDibujo,
                NombreCloud = dibujo.NombreCloud,
                Identificador = dibujo.Identificador,
                TiempoInicio = (int)dibujo.TiempoInicio,
                TiempoFin = (int)dibujo.TiempoFin
            };
        }

        public static List<DibujosVideoResponse> LDibujoALDibujosVideoResponse(List<Dibujos> dibujos)
        {
            List<DibujosVideoResponse> dibujosResponse = new List<DibujosVideoResponse>();
            foreach (var item in dibujos)
            {
                dibujosResponse.Add(DibujoADibujosVideoResponse(item));
            }
            return dibujosResponse;
        }
    }
}
