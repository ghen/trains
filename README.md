# Trains #

This project is a sample implementation for the test programming problem described bellow.

## Implementation Notes ##

Current implementation follows KISS and Agile paradigms to only address currently known requirements.
This leads to early first release delivery and maintains high cost-effectiveness.

Following assumptions and design considerations were made during the project development:

1. Target framework version is set to .NET Framework 4.6, however required version can be lowered easily to support broader range of clients.
2. Console application is a just one way to interface the core library, which, in order, can be reused/integrated into other applications easily (Windows Service, Web Application, etc.)
3. Core library delegates errors handling and logging to the top-level code. In our case - console application itself.
4. All library methods implemented to be thread-safe. Thus data processing can be parallelized easily for larger input arrays.
5. Based on the routes graph structure knowledge and its known boundaries (26x26), application implementation is focused on speed optimization, rather than memory usage optimization.

## Problem ##

The local commuter railroad services a number of towns in Kiwiland. Because of monetary concerns, all of the tracks are 'one-way'. That is, a route from Kaitaia to Invercargill does not imply the existence of a route from Invercargill to Kaitaia. In fact, even if both of these routes do happen to exist, they are distinct and are not necessarily the same distance!

The purpose of this problem is to help the railroad provide its customers with information about the routes. In particular, you will compute the distance along a certain route, the number of different routes between two towns, and the shortest route between two towns.

## Input ##

A directed graph where a node represents a town and an edge represents a route between two towns. The weighting of the edge represents the distance between the two towns. A given route will never appear more than once, and for a given route, the starting and ending town will not be the same town.

For the test input, the towns are named using the first few letters of the alphabet from A to D. A route between two towns (A to B) with a distance of 5 is represented as AB5.

## Output ##

For test input 1 through 5, if no such route exists, output **"NO SUCH ROUTE"**. Otherwise, follow the route as given; do not make any extra stops!

For example, the first problem means to start at city A, then travel directly to city B (a distance of 5), then directly to city C (a distance of 4).

## Example ##

**[ Tests ]**

1. The distance of the route A-B-C.
2. The distance of the route A-D.
3. The distance of the route A-D-C.
4. The distance of the route A-E-B-C-D.
5. The distance of the route A-E-D.
6. The number of trips starting at C and ending at C with a maximum of 3 stops.
    *(In the sample data below, there are two such trips: C-D-C (2 stops). and C-E-B-C (3 stops)).*
7. The number of trips starting at A and ending at C with exactly 4 stops.
    *(In the sample data below, there are three such trips: A to C (via B,C,D); A to C (via D,C,D); and A to C (via D,E,B))*
8. The length of the shortest route (in terms of distance to travel) from A to C.
9. The length of the shortest route (in terms of distance to travel) from B to B.
10. The number of different routes from C to C with a distance of less than 30.
    *(In the sample data, the trips are: CDC, CEBC, CEBCDC, CDCEBC, CDEBC, CEBCEBC, CEBCEBCEBC).*

**[ Routes Graph ]**

```
    AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7
```

**[ Input Data ]**

```
    dist A-B-C
    dist A-D
    dist A-D-C
    dist A-E-B-C-D
    dist A-E-D
    routes C..C [stops <= 3]
    routes A..C [stops == 4]
    dist A..C
    dist B..B
    routes C..C [dist < 30] 
```

**[ Expected Output ]**

```
    9
    5
    13
    22
    NO SUCH ROUTE
    2
    3
    9
    9
    7
```

**[ Complex Routes Definition Sample ]**

```
    C:\Temp>trains routes.txt /v

    #
    # Routes table
    #
    # AB5, AD5, AE7
    # BC4
    # CD8, CE2
    # DC8, DE6
    # EB3

    #
    # Enter your search command:
    #  * (type 'help' for help)
    #

    routes A..E
    # routes "A..E"
    # A-E (dist: 7; stops: 1)
    # A-D-E (dist: 11; stops: 2)
    # A-B-C-E (dist: 11; stops: 3)
    # A-D-C-E (dist: 15; stops: 3)
    # A-B-C-D-E (dist: 23; stops: 4)
    5

    routes A..E [stops == 4]
    # routes "A..E" [stops == 4]
    # A-E-B-C-E (dist: 16; stops: 4)
    # A-B-C-D-E (dist: 23; stops: 4)
    # A-D-C-D-E (dist: 27; stops: 4)
    3

    routes A..C..E [stops == 4]
    # routes "A..C..E" [stops == 4]
    # A-E-B-C-E (dist: 16; stops: 4)
    # A-B-C-D-E (dist: 23; stops: 4)
    # A-D-C-D-E (dist: 27; stops: 4)
    3

    routes A..C-D..E [stops == 4]
    # routes "A..C-D..E" [stops == 4]
    # A-B-C-D-E (dist: 23; stops: 4)
    # A-D-C-D-E (dist: 27; stops: 4)
    2

    routes A-B..D-E [stops == 4]
    # routes "A-B..D-E" [stops == 4]
    # A-B-C-D-E (dist: 23; stops: 4)
    1

```

# Support #

Send your questions to Eugene (yarshevich [att] gmail [dott] com).
Git: https://github.com/ghen/trains
