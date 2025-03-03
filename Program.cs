using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

class Juego
{
    // Definición del tablero de juego
    static char[,] dungeon = new char[12, 60];
    static Random random = new Random();

    // Posiciones de los jugadores
    static int PlayerX1, PlayerY1;
    static int PlayerX2, PlayerY2;

    // Estadísticas de los jugadores
    static int Speed1 = 100;
    static int Speed2 = 100;
    static int SaludJugador1 = 100;
    static int SaludJugador2 = 100;

    // Sistema de niveles y experiencia
    static int NivelJugador1 = 1;
    static int NivelJugador2 = 1;
    static int ExperienciaJugador1 = 0;
    static int ExperienciaJugador2 = 0;

    // Habilidades y cooldowns de los jugadores
    static Dictionary<string, Action> skillPlayer1 = new Dictionary<string, Action>();
    static Dictionary<string, Action> skillPlayer2 = new Dictionary<string, Action>();
    static Dictionary<string, int> cooldownPlayer1 = new Dictionary<string, int>();
    static Dictionary<string, int> cooldownPlayer2 = new Dictionary<string, int>();

    // Cooldowns de habilidades
    static int TurnosRestantesSkill1 = 0;
    static int TurnosRestantesSkill2 = 0;

    // Turnos restantes por jugador
    static int TurnosRestantes = 1;

    static void Main(string[] args)
    {
        IniciarJuego();
    }

    // Método para reiniciar el estado del juego
    static void ReStartStatusGame()
    {
        Speed1 = 100;
        Speed2 = 100;
        SaludJugador1 = 100;
        SaludJugador2 = 100;
        TurnosRestantesSkill1 = 0;
        TurnosRestantesSkill2 = 0;
        NivelJugador1 = 1;
        NivelJugador2 = 1;
        ExperienciaJugador1 = 0;
        ExperienciaJugador2 = 0;
        skillPlayer1.Clear();
        skillPlayer2.Clear();
        cooldownPlayer1.Clear();
        cooldownPlayer2.Clear();
    }


    // Método principal para iniciar el juego
    static void IniciarJuego()
    {
        ReStartStatusGame();
        SeleccionPlayer();
        CrearTablero();
        ColocarCofres(30);
        ColocarRareCofres(5);
        ColocarTrampas(40);
        ColocarMeta(1);
        ColocarPlayer();

        while (true)
        {
            Console.Clear();
            MostrarDungeon();
            MostrarInstrucciones();

            // Turno del jugador 1
            if (TurnoJugador(true))
            {
                // Turno del jugador 2
                if (!TurnoJugador(false)) break;
            }
            else
            {
                break;
            }

            // Actualizar cooldowns
            if (TurnosRestantesSkill1 > 0) TurnosRestantesSkill1--;
            if (TurnosRestantesSkill2 > 0) TurnosRestantesSkill2--;
        }
    }

    // Método para mostrar instrucciones del juego
    static void MostrarInstrucciones()
    {
        Console.WriteLine("Si es su turno presione H para usar su habilidad");
        Console.WriteLine("Tenga en cuenta que cada personaje tiene un tiempo de enfriamiento diferente así que tenga cuidado ");
        Console.WriteLine("1. Paladín: Tiempo de Enfriamiento: 3 turnos");
        Console.WriteLine("2. Mago: Tiempo de enfriamiento: 3 turnos");
        Console.WriteLine("3. Guerrero: Tiempo de enfriamiento: 8 turnos");
        Console.WriteLine("4. Hechicero: Tiempo de enfriamiento: 10 turnos");
        Console.WriteLine();
        Console.WriteLine("Presione la tecla E para salir");
        Console.WriteLine();
        Console.WriteLine("Simbología del juego:");
        Console.WriteLine("1. # -> Muros los cuales no se pueden atravesar");
        Console.WriteLine("2. C -> Son cofres que traen efectos aleatorios. Confía en tu suerte");
        Console.WriteLine("3. R -> Estos son cofres pero sus efectos son tan radicales que podrían darte una ventaja abrumadora o hacerte perder la partida");
        Console.WriteLine("4. T -> Estas son trampas, su función es hacerte daño si caes en ellas");
        Console.WriteLine("5. P1 -> Es el avatar del jugador 1");
        Console.WriteLine("6. P2 -> Es el avatar del jugador 2");
        Console.WriteLine("7. M -> Es la representación de la meta, te reto a encontrarla");
        Console.WriteLine();
        Console.WriteLine("***********************");
        System.Console.WriteLine("Recuerda y es muy importante, si realizas un movimiento que no este permitido\nSe te restara de tu cantidad de pasos permitidos por turno");
    }

    // Método para la selección de personajes
    static void SeleccionPlayer()
    {
        Console.WriteLine("En las frías tierras de Arnos, habita un lugar llamado la cueva de los malditos");
        Console.WriteLine("En este lugar oscuro 2 valientes pueden adentrarse pero nunca ha podido regresar nadie");
        Console.WriteLine("Se dice que el que logre salir le espera grandes riquezas, fama y fortuna.");
        Console.WriteLine("Y hoy dos hábiles guerreros se adentran en esta cueva, tratando de lograr todos sus deseos");
        Console.WriteLine();
        Console.WriteLine("Elige el jugador 1 primero:");
        Console.WriteLine("Presione el número de su personaje según el orden en que aparece y luego presione enter para seleccionar al personaje");
        Console.WriteLine();
        Console.WriteLine("1. Paladín");
        Console.WriteLine("El poderoso paladín puede regenerar su salud a voluntad");
        Console.WriteLine();  
        Console.WriteLine("2. Mago");
        Console.WriteLine("El mago es capaz de teletransportarse, pero el muy inepto no controla donde");
        Console.WriteLine();
        Console.WriteLine("3. Guerrero");
        Console.WriteLine("El poderoso guerrero causa daño con su gran espada, aunque solo cuando esta al lado de su enemigo");
        Console.WriteLine();
        Console.WriteLine("4. Hechicero");
        Console.WriteLine("El puto amo del juego, lanza una bola de fuego a su enemigo, sin necesidad de verlo, el verdadero puto amo");

        string player1;
        int opcion1;
        string imput = (Console.ReadLine()!);
        if(string.IsNullOrWhiteSpace(imput) || !int.TryParse(imput, out opcion1))
        {
            Console.WriteLine("Su pcion no es válida. Se le asignara el personaje de Ninguno como catigo, IMBECIL");
            opcion1 = 0;
            player1 = AsignarPlayer(opcion1, skillPlayer1, cooldownPlayer1);
        }
        else
        {
            player1 = AsignarPlayer(opcion1, skillPlayer1, cooldownPlayer1);
        }

        string player2;
        int opcion2;
        do
        {
            Console.WriteLine("Jugador 2 escoja a su personaje");
            string imput2 = (Console.ReadLine()!);
            if(string.IsNullOrWhiteSpace(imput2) || int.TryParse(imput2, out opcion2) == false)
            {
                Console.WriteLine("Su opcion no es válida. Se le asignara el personaje de Ninguno como catigo, IMBECIL");
                opcion2 = 0;
                player2 = AsignarPlayer(opcion2, skillPlayer2, cooldownPlayer2);
            }
            else
            {
                player2 = AsignarPlayer(opcion2, skillPlayer2, cooldownPlayer2);
            }
        } while (AsignarPlayer(opcion2, skillPlayer2, cooldownPlayer2) == player1);

        Console.WriteLine($"Personajes seleccionados: \n {player1} \n {AsignarPlayer(opcion2, skillPlayer2, cooldownPlayer2)}");
        System.Threading.Thread.Sleep(2000);
    }

    // Método para asignar personajes y sus habilidades
    static string AsignarPlayer(int opcion, Dictionary<string, Action> skills, Dictionary<string, int> cooldowns)
    {
        switch (opcion)
        {
            case 1:
                skills["Paladín"] = () => {
                    if (skills == skillPlayer1)
                    {
                        SaludJugador1 = Math.Min(100, SaludJugador1 + 20);
                        Console.WriteLine("El Paladín del Jugador 1 ha usado su habilidad de curación.");
                    }
                    else
                    {
                        SaludJugador2 = Math.Min(100, SaludJugador2 + 20);
                        Console.WriteLine("El Paladín del Jugador 2 ha usado su habilidad de curación.");
                    }
                };
                cooldowns["Paladín"] = 3;
                return "Paladín";
            case 2:
                skills["Mago"] = () => {
                    if (skills == skillPlayer1)
                        Teleport(ref PlayerX1, ref PlayerY1, true);
                    else
                        Teleport(ref PlayerX2, ref PlayerY2, false);
                };
                cooldowns["Mago"] = 3;
                return "Mago";
            case 3:
                skills["Guerrero"] = () => { /* Esta habilidad se maneja directamente en UsarHabilidad */ };
                cooldowns["Guerrero"] = 8;
                return "Guerrero";
            case 4:
                skills["Hechicero"] = () => { /* Esta habilidad se maneja directamente en UsarHabilidad */ };
                cooldowns["Hechicero"] = 10;
                return "Hechicero";
            case 55140600:
                skills["Dios"] = () => {};
                cooldowns["Dios"] = 0;
                return "Dios";
            case -1:
                skills["Señor del Tiempo"] = () => {};
                cooldowns["Señor del Tiempo"] = 10;
                return "Señor del Tiempo";
            case -2:
                skills["Viajero Dimensional"] = () => {};
                cooldowns["Viajero Dimensional"] = 16;
                return "Viajero Dimensional";
            case -3:
                skills["Creador Mundial"] = () => {};
                cooldowns["Creador Mundial"] = 20;
                return "Creador Mundial";
            case -4:
                skills["Erudito"] = () => {};
                cooldowns["Erudito"] = 3;
                return "Erudito";
            default:
                skills["Ninguno"] = () => {};
                cooldowns["Ninguno"] = 0;
                return "Ninguno";
        }
    }

    // Habilidad de teletransporte del Mago
    static void Teleport(ref int positionX, ref int positionY, bool isPlayer1)
    {
        do
        {
            positionX = random.Next(dungeon.GetLength(0));
            positionY = random.Next(dungeon.GetLength(1));
        } while (dungeon[positionX, positionY] != ' ');

        Console.WriteLine($"El Mago del Jugador {(isPlayer1 ? "1" : "2")} se ha teletransportado a una ubicación aleatoria del mapa");
        System.Threading.Thread.Sleep(2000);
    }

    // Habilidad de daño del Guerrero
    static void WarriorDamage(bool isPlayer1)
    {
        if (isPlayer1)
        {
            if (Math.Abs(PlayerX1 - PlayerX2) <= 1 && Math.Abs(PlayerY1 - PlayerY2) <= 1)
            {
                int damage = random.Next(50, 100);
                SaludJugador2 -= damage;
                Console.WriteLine($"El Guerrero del Jugador 1 ataca y causa {damage} de daño al Jugador 2!");
                GanarExperiencia(true, damage);
                if (SaludJugador2 <= 0)
                {
                    EndGame("La salud del jugador 2 ha llegado a 0\nEL JUGADOR 1 HA GANADO LA PARTIDA");
                }
            }
            else
            {
                Console.WriteLine("El Jugador 2 está demasiado lejos para atacar.");
            }
        }
        else
        {
            if (Math.Abs(PlayerX1 - PlayerX2) <= 1 && Math.Abs(PlayerY1 - PlayerY2) <= 1)
            {
                int damage = random.Next(40, 61);
                SaludJugador1 -= damage;
                Console.WriteLine($"El Guerrero del Jugador 2 ataca y causa {damage} de daño al Jugador 1!");
                GanarExperiencia(false, damage);
                if (SaludJugador1 <= 0)
                {
                    EndGame("La salud del jugador 1 ha llegado a 0\nEL JUGADOR 2 HA GANADO LA PARTIDA");
                }
            }
            else
            {
                Console.WriteLine("El Jugador 1 está demasiado lejos para atacar.");
            }
        }
        System.Threading.Thread.Sleep(2000);
    }

    // Habilidad de daño del Hechicero
    static void WhitcherDamage(bool isPlayer1)
    {
        if (isPlayer1)
        {
            int damage = random.Next(10, 21);
            SaludJugador2 -= damage;
            Console.WriteLine($"El Hechicero del Jugador 1 lanza un hechizo y causa {damage} de daño al Jugador 2!");
            GanarExperiencia(true, damage);
            if (SaludJugador2 <= 0)
            {
                EndGame("La salud del jugador 2 ha llegado a 0\nEL JUGADOR 1 HA GANADO LA PARTIDA");
            }
        }
        else
        {
            int damage = random.Next(10, 21);
            SaludJugador1 -= damage;
            Console.WriteLine($"El Hechicero del Jugador 2 lanza un hechizo y causa {damage} de daño al Jugador 1!");
            GanarExperiencia(false, damage);
            if (SaludJugador1 <= 0)
            {
                EndGame("La salud del jugador 1 ha llegado a 0\nEL JUGADOR 2 HA GANADO LA PARTIDA");
            }
        }
        System.Threading.Thread.Sleep(2000);
    }

    static void NoneSkill(bool isPlayer1)
    {
        if(isPlayer1)
        {
            Console.WriteLine("YA DIJE QUE NO TIENEN NINGUNA HABILIDAD, POR IMBECIL TUS STATS HAN SIDO REDUCIDOS");
            System.Threading.Thread.Sleep(2000);
            int damage = random.Next(20, 51);
            int speedreduce = random.Next(25, 76);
            SaludJugador1 -= damage;
            Speed1 -= speedreduce;
        }
        else
        {
            Console.WriteLine("YA DIJE QUE NO TIENEN NINGUNA HABILIDAD, POR IMBECIL TUS STATS HAN SIDO REDUCIDOS");
            System.Threading.Thread.Sleep(2000);
            int damage = random.Next(20, 51);
            int speedreduce = random.Next(25, 76);
            SaludJugador2 -= damage;
            Speed2 -= speedreduce;
        }
    }

    static void GodSkill(bool isPlayer1)
    {
        if(isPlayer1)
        {
            Console.WriteLine("SOLO LOS DIOSES TIENEN PERMITIDO HACER LO QUE QUIERAN");
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine("NO SE PUEDE COMPETIR CONTRA EL CREADOR DE TODO");
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine("CONOCE LA FURIA DE DIOS");
            System.Threading.Thread.Sleep(2000);
            EndGame("Se ha sucumbido a la ira de DIOS \nEl jugador 1 ha ganado la partida");
        }
        else
        {
            Console.WriteLine("SOLO LOS DIOSES TIENEN PERMITIDO HACER LO QUE QUIERAN");
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine("NO SE PUEDE COMPETIR CONTRA EL CREADOR DE TODO");
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine("CONOCE LA FURIA DE DIOS");
            System.Threading.Thread.Sleep(2000);
            EndGame("Se ha sucumbido a la ira de DIOS \nEl jugador 2 ha ganado la partida");
        }
    }

    static void RevelarMeta()
    {
        Console.WriteLine("El Erudito revela la posicion de la meta.");
        int metaX = -1, metaY = -1;
        for (int i = 0; i < dungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dungeon.GetLength(1); j++)
            {
                if (dungeon[i, j] == 'M')
                {
                    metaX = i;
                    metaY = j;
                    break;
                }
            }
            if (metaX != -1) break;
        }
        Console.WriteLine("Recuerda que la mazmorra comienza desde las coordenadas 0,0");
        Console.WriteLine($"La meta esta en la posicion ({metaX}, {metaY})");
        System.Threading.Thread.Sleep(8000);
    }

    static void TeleportToDugeon()
    {
        Console.WriteLine("El Viajero Dimensional transporta a los jugadores a una nueva mazmorra.");
        System.Threading.Thread.Sleep(2000);
        Console.Clear();
        CrearTablero();
        ColocarCofres(30);
        ColocarRareCofres(5);
        ColocarTrampas(40);
        ColocarMeta(1);
        ColocarPlayer();
    }

    static void RestartGame()
    {
        Console.WriteLine("El Creador Mundial es un ser omnipotente, ha regresado el curso del tiempo y ahora el mundo se reiniciara, y cambiara");
        System.Threading.Thread.Sleep(2000);
        IniciarJuego();
    }

    static void ResetPlayerStats()
    {
        Console.WriteLine("El Señor del tiempo tiene  el poder de reiniciar la fuerza de los jugadores");
        System.Threading.Thread.Sleep(2000);

            Speed1 = 100;
            Speed2 = 100;
            SaludJugador1 = 100;
            SaludJugador2 = 100;
            NivelJugador1 = 1;
            NivelJugador2 = 1;
            ExperienciaJugador1 = 0;
            ExperienciaJugador2 = 0;
    }

    

    // Método para crear el tablero de juego
    static void CrearTablero()
    {
        for (int i = 0; i < dungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dungeon.GetLength(1); j++)
            {
                dungeon[i, j] = '#';
            }
        }

        GenerarLaberinto(1, 1);
    }

    // Método para generar el laberinto usando el algoritmo de backtracking
    static void GenerarLaberinto(int x, int y)
    {
        dungeon[x, y] = ' ';

        List<(int, int)> direcciones = new List<(int, int)> { (-2, 0), (2, 0), (0, -2), (0, 2) };
        direcciones = direcciones.OrderBy(d => random.Next()).ToList();

        foreach (var (dx, dy) in direcciones)
        {
            int nx = x + dx, ny = y + dy;
            if (nx > 0 && nx < dungeon.GetLength(0) - 1 && ny > 0 && ny < dungeon.GetLength(1) - 1 && dungeon[nx, ny] == '#')
            {
                dungeon[x + dx / 2, y + dy / 2] = ' ';
                GenerarLaberinto(nx, ny);
            }
        }
    }

    // Método para colocar cofres en el laberinto
    static void ColocarCofres(int cantidadCofres)
    {
        List<(int, int)> cofres = new List<(int, int)>();
        for (int i = 0; i < dungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dungeon.GetLength(1); j++)
            {
                if (dungeon[i, j] == ' ')
                    cofres.Add((i, j));
            }
        }

        for (int i = 0; i < cantidadCofres; i++)
        {
            if (cofres.Count == 0) break;
            int index = random.Next(cofres.Count);
            var (x, y) = cofres[index];
            dungeon[x, y] = 'C';
            cofres.RemoveAt(index);
        }
    }

    // Método para procesar el efecto de un cofre normal
    static bool ProcesamientoCofre(bool isPlayer1)
    {
        int action = random.Next(4);

        switch (action)
        {
            case 0:
                if (isPlayer1)
                {
                    Speed1 += 5;
                    Console.WriteLine("La velocidad del jugador 1 ha aumentado");
                }
                else
                {
                    Speed2 += 5;
                    Console.WriteLine("La velocidad del jugador 2 ha aumentado");
                }
                break;
            case 1:
                if (isPlayer1)
                {
                    Speed1 = Math.Max(1, Speed1 - 5);
                    Console.WriteLine("La velocidad del jugador 1 ha disminuido");
                }
                else
                {
                    Speed2 = Math.Max(1, Speed2 - 5);
                    Console.WriteLine("La velocidad del jugador 2 ha disminuido");
                }
                break;
            case 2:
                if (isPlayer1)
                {
                    Console.WriteLine("El jugador 1 ha encontrado un mímico");
                    SaludJugador1 -= 10;
                    Console.WriteLine("El mímico ha causado daño al jugador y luego ha desaparecido");
                }
                else
                {
                    Console.WriteLine("El jugador 2 ha encontrado un mímico");
                    SaludJugador2 -= 10;
                    Console.WriteLine("El mímico ha causado daño al jugador y luego ha desaparecido");
                }
                break;
            case 3:
                if (isPlayer1)
                {
                    SaludJugador1 = Math.Min(100, SaludJugador1 + 20);
                    Console.WriteLine("La salud del jugador 1 ha aumentado");
                }
                else
                {
                    SaludJugador2 = Math.Min(100, SaludJugador2 + 20);
                    Console.WriteLine("La salud del jugador 2 ha aumentado");
                }
                break;
        }

        GanarExperiencia(isPlayer1, 50);
        System.Threading.Thread.Sleep(2000);
        return CheckVictoryCondition();
    }

    // Método para colocar cofres raros en el laberinto
    static void ColocarRareCofres(int CantidadCofres)
    {
        List<(int, int)> Cofres = new List<(int, int)>();
        for (int i = 0; i < dungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dungeon.GetLength(1); j++)
            {
                if (dungeon[i, j] == ' ')
                    Cofres.Add((i, j));
            }
        }

        for (int i = 0; i < CantidadCofres; i++)
        {
            if (Cofres.Count == 0) break;
            int index = random.Next(Cofres.Count);
            var (x, y) = Cofres[index];
            dungeon[x, y] = 'R';
            Cofres.RemoveAt(index);
        }
    }

    // Método para procesar el efecto de un cofre raro
    static bool ProcesamientoRareCofre(bool isplayer1)
    {
        int Action = random.Next(4);

        switch (Action)
        {
            case 0:
                if (isplayer1)
                {
                    Speed1 += 50;
                    Console.WriteLine("La velocidad del jugador 1 ha aumentado significativamente");
                }
                else
                {
                    Speed2 += 50;
                    Console.WriteLine("La velocidad del jugador 2 ha aumentado significativamente");
                }
                break;
            case 1:
                if (isplayer1)
                {
                    Speed1 = Math.Max(1, Speed1 - 25);
                    Console.WriteLine("La velocidad del jugador 1 ha disminuido considerablemente");
                }
                else
                {
                    Speed2 = Math.Max(1, Speed2 - 25);
                    Console.WriteLine("La velocidad del jugador 2 ha disminuido considerablemente");
                }
                break;
            case 2:
                if (isplayer1)
                {
                    Console.WriteLine("El jugador 1 ha encontrado un mímico tenebroso");
                    SaludJugador1 -= 70;
                    Console.WriteLine("El mímico tenebroso ha causado un daño severo al jugador y luego ha desaparecido");
                }
                else
                {
                    Console.WriteLine("El jugador 2 ha encontrado un mímico tenebroso");
                    SaludJugador2 -= 70;
                    Console.WriteLine("El mímico tenebroso ha causado un daño severo al jugador y luego ha desaparecido");
                }
                break;
            case 3:
                if (isplayer1)
                {
                    SaludJugador1 = Math.Min(100, SaludJugador1 + 80);
                    Console.WriteLine("La salud del jugador 1 ha aumentado considerablemente");
                }
                else
                {
                    SaludJugador2 = Math.Min(100, SaludJugador2 + 80);
                    Console.WriteLine("La salud del jugador 2 ha aumentado considerablemente");
                }
                break;
        }

        GanarExperiencia(isplayer1, 100);
        System.Threading.Thread.Sleep(2000);
        return CheckVictoryCondition();
    }

    // Método para colocar trampas en el laberinto
    static void ColocarTrampas(int cantidadTrap)
    {
        List<(int, int)> trampas = new List<(int, int)>();

        for (int i = 0; i < dungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dungeon.GetLength(1); j++)
            {
                if (dungeon[i, j] == ' ')
                {
                    trampas.Add((i, j));
                }
            }
        }

        for (int i = 0; i < cantidadTrap; i++)
        {
            if (trampas.Count == 0) break;
            int index = random.Next(trampas.Count);
            var (x, y) = trampas[index];
            dungeon[x, y] = 'T';
            trampas.RemoveAt(index);
        }
    }

    // Método para colocar la meta en el laberinto
    static void ColocarMeta(int meta)
    {
        List<(int, int)> metas = new List<(int, int)>();
        for (int i = 0; i < dungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dungeon.GetLength(1); j++)
            {
                if (dungeon[i, j] == ' ')
                {
                    metas.Add((i, j));
                }
            }
        }

        for (int i = 0; i < meta; i++)
        {
            if (metas.Count == 0) break;
            int index = random.Next(metas.Count);
            var (x, y) = metas[index];
            dungeon[x, y] = 'M';
            metas.RemoveAt(index);
        }
    }

    // Método para colocar a los jugadores en el laberinto
    static void ColocarPlayer()
    {
        do
        {
            PlayerX1 = random.Next(dungeon.GetLength(0));
            PlayerY1 = random.Next(dungeon.GetLength(1));
        } while (dungeon[PlayerX1, PlayerY1] != ' ');

        do
        {
            PlayerX2 = random.Next(dungeon.GetLength(0));
            PlayerY2 = random.Next(dungeon.GetLength(1));
        } while ((PlayerX2 == PlayerX1 && PlayerY2 == PlayerY1) || dungeon[PlayerX2, PlayerY2] != ' ');
    }

    // Método para mostrar el laberinto en la consola
    static void MostrarDungeon()
    {
        Console.Clear();
        for (int i = 0; i < dungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dungeon.GetLength(1); j++)
            {
                if (Math.Abs(PlayerX1 - i) <= 1 && Math.Abs(PlayerY1 - j) <= 1)
                {
                    if (PlayerX1 == i && PlayerY1 == j)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("P1");
                        Console.ResetColor();
                    }
                    else
                    {
                        MostrarElemento(dungeon[i, j]);
                    }
                }
                else if (Math.Abs(PlayerX2 - i) <= 1 && Math.Abs(PlayerY2 - j) <= 1)
                {
                    if (PlayerX2 == i && PlayerY2 == j)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("P2");
                        Console.ResetColor();
                    }
                    else
                    {
                        MostrarElemento(dungeon[i, j]);
                    }
                }
                else
                {
                    Console.Write(". ");
                }
            }
            Console.WriteLine();
        }
        
        MostrarEstadisticasJugadores();
    }

    // Método para mostrar elementos del laberinto con colores
    static void MostrarElemento(char elemento)
    {
        switch (elemento)
        {
            case '#':
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("# ");
                break;
            case 'C':
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("C ");
                break;
            case 'R':
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("R ");
                break;
            case 'T':
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("T ");
                break;
            case 'M':
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("M ");
                break;
            default:
                Console.Write(elemento + " ");
                break;
        }
        Console.ResetColor();
    }

    // Método para mostrar las estadísticas de los jugadores
    static void MostrarEstadisticasJugadores()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Jugador 1 - Nivel: {NivelJugador1}, Experiencia: {ExperienciaJugador1}/{NivelJugador1*100}, Velocidad: {Speed1}, Salud: {SaludJugador1}");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Jugador 2 - Nivel: {NivelJugador2}, Experiencia: {ExperienciaJugador2}/{NivelJugador2*100}, Velocidad: {Speed2}, Salud: {SaludJugador2}");
        Console.ResetColor();
    }

    // Método para manejar el turno de un jugador
    static bool TurnoJugador(bool Player1)
    {
        int maxMove = TurnosRestantes + (Player1 ? Speed1 : Speed2) / 25;
        
        Console.WriteLine($"Es el turno del {(Player1 ? "Jugador 1" : "Jugador 2")}");
        if (Player1)
        {
            Console.WriteLine("Para mover al jugador 1 presione WSAD");
            Console.WriteLine("W -> Arriba\nS -> Abajo\nA -> Izquierda\nD ->Derecha");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Para mover al jugador 2 presione 1234");
            Console.WriteLine("1 -> Arriba\n2 -> Abajo\n3 -> Izquierda\n4 -> Derecha");
            Console.WriteLine();
        }
        Console.WriteLine($"Solo es posible moverte {maxMove} casillas");
        
        Console.WriteLine("Presiona una de las teclas determinadas para moverte o presiona E para salir");
        
        for(int RealizeMove = 0; RealizeMove < maxMove; RealizeMove++)
        {
            var Tecla = Console.ReadKey(true).Key;

            if (Tecla == ConsoleKey.E)
                return false;

                if (Player1)
                {
                    switch (Tecla)
                    {
                        case ConsoleKey.W: Move(-1, 0, true); break;
                        case ConsoleKey.S: Move(1, 0, true); break;
                        case ConsoleKey.A: Move(0, -1, true); break;
                        case ConsoleKey.D: Move(0, 1, true); break;
                        case ConsoleKey.H:
                            if (TurnosRestantesSkill1 == 0)
                            {
                                UsarHabilidad(true);
                            }
                            else
                            {
                                Console.WriteLine($"No es posible usar la habilidad en este momento. Pasos restantes: {TurnosRestantesSkill1}");
                                System.Threading.Thread.Sleep(2000);
                            }
                            break;
                    }

                    MostrarDungeon();
                } 
                else 
                { 
                    switch (Tecla)
                    { 
                        case ConsoleKey.D1: Move(-1, 0, false); break;
                        case ConsoleKey.D2: Move(1, 0, false); break;
                        case ConsoleKey.D3: Move(0, -1, false); break;
                        case ConsoleKey.D4: Move(0, 1, false); break;
                        case ConsoleKey.H:
                            if (TurnosRestantesSkill2 == 0)
                            {
                                UsarHabilidad(false);
                            }
                            else
                            {
                                Console.WriteLine($"No es posible usar la habilidad en este momento. Pasos restantes: {TurnosRestantesSkill2}");
                                System.Threading.Thread.Sleep(2000);
                            }
                            break;
                    }
                    MostrarDungeon();
                } 
        } 
        return true; 
    }

    // Método para usar la habilidad del personaje
    static void UsarHabilidad(bool isPlayer1)
    {
        if (isPlayer1)
        {
            if (skillPlayer1.ContainsKey("Paladín"))
            {
                Console.WriteLine("El jugador 1 ha usado su habilidad de Paladín");
                skillPlayer1["Paladín"]();
                TurnosRestantesSkill1 = cooldownPlayer1["Paladín"];
            }
            else if (skillPlayer1.ContainsKey("Mago"))
            {
                Console.WriteLine("El jugador 1 ha usado su habilidad de Mago");
                skillPlayer1["Mago"]();
                TurnosRestantesSkill1 = cooldownPlayer1["Mago"];
            }
            else if (skillPlayer1.ContainsKey("Guerrero"))
            {
                Console.WriteLine("El jugador 1 ha usado su habilidad de Guerrero");
                WarriorDamage(true);
                TurnosRestantesSkill1 = cooldownPlayer1["Guerrero"];
            }
            else if (skillPlayer1.ContainsKey("Hechicero"))
            {
                Console.WriteLine("El jugador 1 ha usado su habilidad de Hechicero");
                WhitcherDamage(true);
                TurnosRestantesSkill1 = cooldownPlayer1["Hechicero"];
            }
            else if(skillPlayer1.ContainsKey("Dios"))
            {
                Console.WriteLine("El jugador 1 ha usado su habilidad");
                GodSkill(true);
                TurnosRestantesSkill1 = cooldownPlayer1["Dios"];
            }
            else if(skillPlayer1.ContainsKey("Ninguno"))
            {
                Console.WriteLine("TE JURO QUE NO HE CONOCIDO GENTE MAS ESTUPIDA QUE TU");
                System.Threading.Thread.Sleep(2000);
                NoneSkill(true);
                TurnosRestantesSkill1 = cooldownPlayer1["Ninguno"];
            }
            else if(skillPlayer1.ContainsKey("Señor del Tiempo"))
            {
                Console.WriteLine("El jugador 1 ha usado su habilidad");
                ResetPlayerStats();
                TurnosRestantesSkill1 = cooldownPlayer1["Señor del Tiempo"];
            }
            else if(skillPlayer1.ContainsKey("Erudito"))
            {
                Console.WriteLine("El jugador 1 ha usado su habilidad");
                RevelarMeta();
                TurnosRestantesSkill1 = cooldownPlayer1["Erudito"];
            }
            else if(skillPlayer1.ContainsKey("Creador Mundial"))
            {
                Console.WriteLine("El jugador 1 ha usado su habilidad");
                RestartGame();
                TurnosRestantesSkill1 = cooldownPlayer1["Creador Mundial"];
            }
            else if(skillPlayer1.ContainsKey("Viajero Dimensional"))
            {
                Console.WriteLine("El jugador 1 ha usado su habilidad");
                TeleportToDugeon();
                TurnosRestantesSkill1 = cooldownPlayer1["Viajero Dimensional"];
            }
        }
        else
        {
            if (skillPlayer2.ContainsKey("Paladín"))
            {
                Console.WriteLine("El jugador 2 ha usado su habilidad de Paladín");
                skillPlayer2["Paladín"]();
                TurnosRestantesSkill2 = cooldownPlayer2["Paladín"];
            }
            else if (skillPlayer2.ContainsKey("Mago"))
            {
                Console.WriteLine("El jugador 2 ha usado su habilidad de Mago");
                skillPlayer2["Mago"]();
                TurnosRestantesSkill2 = cooldownPlayer2["Mago"];
            }
            else if (skillPlayer2.ContainsKey("Guerrero"))
            {
                Console.WriteLine("El jugador 2 ha usado su habilidad de Guerrero");
                WarriorDamage(false);
                TurnosRestantesSkill2 = cooldownPlayer2["Guerrero"];
            }
            else if (skillPlayer2.ContainsKey("Hechicero"))
            {
                Console.WriteLine("El jugador 2 ha usado su habilidad de Hechicero");
                WhitcherDamage(false);
                TurnosRestantesSkill2 = cooldownPlayer2["Hechicero"];
            }
            else if(skillPlayer2.ContainsKey("Dios"))
            {
                Console.WriteLine("El jugador 2 ha usado su habilidad");
                GodSkill(false);
                TurnosRestantesSkill2 = cooldownPlayer2["Dios"];
            }
            else if(skillPlayer2.ContainsKey("Ninguno"))
            {
                Console.WriteLine("TE JURO QUE NO HE CONOCIDO GENTE MAS ESTUPIDA QUE TU");
                System.Threading.Thread.Sleep(2000);
                NoneSkill(false);
                TurnosRestantesSkill2 = cooldownPlayer2["Ninguno"];
            }
            else if(skillPlayer2.ContainsKey("Señor del Tiempo"))
            {
                Console.WriteLine("El jugador 2 ha usado su habilidad");
                ResetPlayerStats();
                TurnosRestantesSkill2 = cooldownPlayer2["Señor del Tiempo"];
            }
            else if(skillPlayer2.ContainsKey("Erudito"))
            {
                Console.WriteLine("El jugador 2 ha usado su habilidad");
                RevelarMeta();
                TurnosRestantesSkill2 = cooldownPlayer2["Erudito"];
            }
            else if(skillPlayer2.ContainsKey("Creador Mundial"))
            {
                Console.WriteLine("El jugador 2 ha usado su habilidad");
                RestartGame();
                TurnosRestantesSkill2 = cooldownPlayer2["Creador Mundial"];
            }
            else if(skillPlayer2.ContainsKey("Viajero Dimensional"))
            {
                Console.WriteLine("El jugador 2 ha usado su habilidad");
                TeleportToDugeon();
                TurnosRestantesSkill2 = cooldownPlayer2["Viajero Dimensional"];
            }
        }
    }

    // Método para verificar si un movimiento es válido
    static bool ValidMove(int newX, int newY)
    {
        return newX >= 0 && newX < dungeon.GetLength(0) &&
               newY >= 0 && newY < dungeon.GetLength(1) &&
               !(newX == PlayerX1 && newY == PlayerY1) &&
               !(newX == PlayerX2 && newY == PlayerY2) &&
               dungeon[newX, newY] != '#';
    }

    // Método para mover al jugador
    static void Move(int posicionX, int posicionY, bool Player1)
    {
        int NewPlayerX = Player1 ? PlayerX1 + posicionX : PlayerX2 + posicionX;
        int NewPlayerY = Player1 ? PlayerY1 + posicionY : PlayerY2 + posicionY;

            if (ValidMove(NewPlayerX, NewPlayerY))
            {
                if (dungeon[NewPlayerX, NewPlayerY] == 'M')
                {
                    if (Player1)
                    {
                        EndGame("El jugador 1 ha llegado a la salida del laberinto\nEl Jugador 1 ha ganado la partida");
                    }
                    else
                    {
                        EndGame("El jugador 2 ha llegado a la salida del laberinto\nEl jugador 2 ha ganado la partida");
                    }
                }

                if (dungeon[NewPlayerX, NewPlayerY] == 'T')
                {
                    int DanioTrap = random.Next(1, 31);
                    if (Player1)
                    {
                        SaludJugador1 -= DanioTrap;
                        if (SaludJugador1 <= 0)
                        {
                            SaludJugador1 = 0;
                            EndGame("La salud del jugador 1 ha llegado a 0\nEl jugador 2 ha ganado la partida");
                        }
                    }
                    else
                    {
                        SaludJugador2 -= DanioTrap;
                        if (SaludJugador2 <= 0)
                        {
                            SaludJugador2 = 0;
                            EndGame("La salud del jugador 2 ha llegado a 0\nEl jugador 1 ha ganado la partida");
                        }
                    }

                    Console.WriteLine($"{(Player1 ? "Jugador 1" : "Jugador 2")} se ha encontrado una trampa! Ha recibido daño como consecuencia");
                    Console.WriteLine($"Salud Restante del {(Player1 ? "Jugador 1" : "Jugador 2")} es de {(Player1 ? SaludJugador1 : SaludJugador2)}");
                    System.Threading.Thread.Sleep(2000);
                }

                if (dungeon[NewPlayerX, NewPlayerY] == 'C')
                {
                    ProcesamientoCofre(Player1);
                    dungeon[NewPlayerX, NewPlayerY] = ' ';
                }

                if (dungeon[NewPlayerX, NewPlayerY] == 'R')
                {
                    ProcesamientoRareCofre(Player1);
                    dungeon[NewPlayerX, NewPlayerY] = ' ';
                }

                if (Player1)
                {
                    PlayerX1 = NewPlayerX;
                    PlayerY1 = NewPlayerY;
                }
                else
                {
                    PlayerX2 = NewPlayerX;
                    PlayerY2 = NewPlayerY;
                }

                ReproducirSonido("movimiento");
            }
        }

    // Método para finalizar el juego
    static void EndGame(string Message)
    {
        ReproducirSonido("victoria");
        Console.Clear();
        Console.WriteLine(Message);
        Console.WriteLine("Que deseas hacer a continuación:");
        Console.WriteLine("1. Volver a Jugar");
        Console.WriteLine("2. Salir");
        
        while (true)
        {
            var botton = Console.ReadKey(true).KeyChar;
            if (botton == '1')
            {
                IniciarJuego();
                return;
            }
            else if (botton == '2')
            {
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("\nOpción no válida. Por favor, elige 1 o 2.");
                Console.WriteLine("Presiona cualquier tecla para continuar...");
                Console.ReadKey(true);
                Console.WriteLine("Que deseas hacer a continuación:");
                Console.WriteLine("1. Volver a Jugar");
                Console.WriteLine("2. Salir");
            }
        }
    }

    // Método para ganar experiencia
    static void GanarExperiencia(bool esJugador1, int cantidad)
    {
        if (esJugador1)
        {
            ExperienciaJugador1 += cantidad;
            if (ExperienciaJugador1 >= NivelJugador1 * 100)
            {
                NivelJugador1++;
                ExperienciaJugador1 -= (NivelJugador1 - 1) * 100;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"¡Jugador 1 ha subido al nivel {NivelJugador1}!");
                Console.ResetColor();
                SaludJugador1 += 40;
                Speed1 += 25;
            }
        }
        else
        {
            ExperienciaJugador2 += cantidad;
            if (ExperienciaJugador2 >= NivelJugador2 * 100)
            {
                NivelJugador2++;
                ExperienciaJugador2 -= (NivelJugador2 - 1) * 100;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"¡Jugador 2 ha subido al nivel {NivelJugador2}!");
                Console.ResetColor();
                SaludJugador2 += 40;
                Speed2 += 25;
            }
        }
    }

    // Método para reproducir sonidos
    static void ReproducirSonido(string sonido)
    {
        try
        {
            if (!OperatingSystem.IsWindows())
            {
                Console.WriteLine("Los sonidos solo están disponibles en Windows.");
                return;
            }

            switch (sonido)
            {
                case "movimiento":
                    Console.Beep(440, 100);
                    break;
                case "cofre":
                    Console.Beep(660, 100);
                    break;
                case "trampa":
                    Console.Beep(220, 300);
                    break;
                case "victoria":
                    for (int i = 0; i < 3; i++)
                    {
                        Console.Beep(440, 100);
                        Console.Beep(660, 100);
                    }
                    break;
                default:
                    Console.WriteLine($"Sonido desconocido: {sonido}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al reproducir sonido: {ex.Message}");
        }
    }

    // Método para verificar la condición de victoria
    static bool CheckVictoryCondition()
    {
        if (SaludJugador1 <= 0)
        {
            EndGame("La salud del jugador 1 ha llegado a 0\nEL JUGADOR 2 HA GANADO LA PARTIDA");
            return true;
        }
        else if (SaludJugador2 <= 0)
        {
            EndGame("La salud del jugador 2 ha llegado a 0\nEL JUGADOR 1 HA GANADO LA PARTIDA");
            return true;
        }
        return false;
    }
}