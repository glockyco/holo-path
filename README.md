# Indoor-Wegfindung mit der HoloLens

## Inhaltsverzeichnis

-   <a href="#problemstellung">Problemstellung</a>
-   <a href="#implementierung">Lösungsansatz und Implementierung</a>
    -   <a href="#positionsbestimmung">Positionsbestimmung</a>
    -   <a href="#spracheingabe">Spracheingabe</a>
    -   <a href="#wegfindung">Wegfindung</a>
    -   <a href="#routenanzeige">Anzeige der Route</a>
    -   <a href="#sonstiges">Sonstige Implementierungsdetails</a>
-   <a href="#diskussion">Diskussion der Ergebnisse</a>

<a id="problemstellung"></a>
## Problemstellung

Ziel dieses Projekts war es, eine HoloLens-Applikation zu entwickeln,
die den Einsatz der HoloLens als "Indoor-Navigationsgerät" ermöglicht.

Konkret soll ein Benutzer der Applikation mittels Spracheingabe (z.B.
"Navigate to Room 301") den Wegfindungs-Prozess starten können. Die
Applikation soll auf diesen Befehl hin eine Route von der aktuellen
Position des Benutzers zu seiner gewünschten Zielposition berechnen
und über die HoloLens eine entsprechende AR-Linie anzeigen, welcher
der Benutzer zur Zielposition folgen kann. Während sich der Benutzer
bewegt, soll die errechnet Route regelmäßig aktualisiert werden, um
den Benutzer auch dann erfolgreich zum Ziel zu lotsen, wenn er vom
ursprünglichen Pfad abkommt.

<a id="implementierung"></a>
## Lösungsansatz und Implementierung

Um die gegebene Problemstellung zu lösen, werden im Wesentlichen
folgende Komponenten benötigt:

-   <a href="#positionsbestimmung">Positionsbestimmung</a>
-   <a href="#spracheingabe">Spracheingabe</a>
-   <a href="#wegfindung">Wegfindung</a>
-   <a href="#routenanzeige">Anzeige der Route</a>

Details zu diesen Komponenten werden in den folgenden Abschnitten
beschrieben.

<a id="positionsbestimmung"></a>
### Positionsbestimmung

Die Position des Benutzers (eigentlich: der HoloLens) relativ zu seiner
Position beim Starten der Applikation ist in Unity über die Position der
Main Camera gegeben, die automatisch mit dem Benutzer mitbewegt und an
seine Orientierung angepasst wird.

Diese Information alleine reicht allerdings nicht aus, um eine Aussage
darüber treffen zu können, wo sich der Benutzer gerade im Gebäude
befindet - zumindest wenn man davon ausgeht, dass die Applikation nicht
immer an der genau gleichen physikalischen Position und mit genau
gleicher Orientierung gestartet wird, d.h. der Ursprung und die Rotation
des virtuellen Koordinatensystems bei jedem Start verschieden sind.

Um die Position des Gebäudes relativ zum Ursprung zu bestimmen, wird
deshalb ein Vuforia Image Target (https://engine.vuforia.com/content/vuforia/en/features.html)
als Marker verwendet. In einem echten Anwendungsfall könnte das z.B. ein
Raumplan oder ein anderes Objekt im Gebäude sein. Wichtig ist nur, dass
der Marker eine fixe Position im Gebäude hat und nicht mehrere "Kopien"
davon existieren, damit bei Erkennen des Markers eine eindeutige Aussage
darüber getroffen werden kann, wo sich der Benutzer gerade im Gebäude
befindet.

In der Implementierung muss dann nur das virtuelle Gebäudemodell richtig
an den Marker "angehängt" werden, d.h. Kindelement des Markers sein und
in Unity so positioniert werden, dass der virtuelle Marker sich an der
richtigen Position im virtuellen Gebäudemodell befindet. Vuforia kümmert
sich dann darum, dass der virtuelle Marker (und damit auch das daran
angehängte Gebäudemodell) richtig im virtuellen Raum positioniert wird,
wenn der physikalische Marker erkannt wird. Die Position des Benutzers
im Gebäude ist somit ab der ersten Marker-Erkennung klar bestimmt.

![Vuforia Image Target](./Images/image_target.png)



<a id="spracheingabe"></a>
### Spracheingabe

Für die Definition der zu erkennenden Sprachbefehle sowie der Aktionen,
die bei Erkennen eines Befehls ausgeführt werden sollen, wird auf die
entsprechenden Funktionalitäten des Mixed Reality Toolkit (MRTK) von
Microsoft zurückgegriffen (https://github.com/microsoft/MixedRealityToolkit-Unity).

Definiert wird pro Raum ein Sprachbefehl ("Navigate to Room X"), der 
die Zielposition des Wegfindungs-Algorithmus auf den genannten Raum
setzt.


![Speech Command Definition](./Images/speech_command_definition.png)

![Speech Command Handling](./Images/speech_command_handling.png)

<a id="wegfindung"></a>
### Wegfindung

Für die Wegfindung wird das A* Pathfinding Project von Aron Granberg
verwendet (https://arongranberg.com/astar). Das Navigation System von
Unity (https://docs.unity3d.com/Manual/nav-NavigationSystem.html) bietet
zwar sehr ähnliche Funktionalitäten, kann aber von der Performanz (die
insbesondere mit den begrenzten Ressourcen der HoloLens durchaus
relevant ist) und vom Feature-Umfang her nicht mit der A* Library von
Granberg mithalten, weshalb dieser der Vorzug gegeben wurde.

Bevor die A* Library eine Route berechnen kann, muss sie einen
sogennanten Recast Graph aufbauen, der die begehbaren Flächen bzw.
Wege beschreibt (die Library unterstützt auch andere Repräsentationen;
für das Projekt war der Recast Graph aber am besten geeignet).
Standardmäßig wird dieser Graph auf Basis aller Objekt-Meshes erzeugt,
die beim Applikationsstart in der Szenen aktiv sind, wobei z.B.
eingestellt werden kann, wie groß eine Fläche mindestens sein muss, um
als begehbar zu gelten, was die maximale begehbare Steigung ist, welche
Objekt-Meshes bei der Berechnung des Graphen ignoriert werden sollen,
etc. Damit der Graph nicht bei jedem Applikationsstart neu berechnet
werden muss, bietet die Library außerdem die Möglichkeit, den Graphen
in einer Datei zu cachen und von dort zu laden.

![Recast Graph](./Images/recast_graph.png)

Um eine Route vom Benutzer zur mittels Spracheingabe ausgewählten
Zielposition zu erhalten, wird eine `RichAI` Komponente der A* Library
an die Main Camera (d.h. den Benutzer) angehängt. Die Komponente bringt
im Wesentlichen bereits alle Wegfindungs-Funktionalitäten mit, die im
Projekt benötigt werden. Eine Route von der aktuellen Position zu einer
bestimmten Zielposition kann in einem frei definierbaren Intervall
berechnet und sehr einfach als Liste von Wegpunkten aus der Komponente
ausgelesen werden. Nur die Bewegungslogik, die eigentlich dafür gedacht
ist, AI-Agenten automatisiert zur Zielposition marschieren zu lassen,
muss deaktiviert werden, was aber sehr einfach über eine Checkbox
möglich ist.

![Local Space Rich AI](./Images/local_space_rich_ai.png)

Weil der Graph auf Basis der initialen Positionen aller Objekte erstellt
wird, ergibt sich noch das Problem, dass er nach der Marker-basierten
Neupositionierung des Gebäudemodells nicht mehr mit dem Gebäudemodell
übereinstimmt. Dieses Problem wird dadurch gelöst, dass in der
Implementierung alle von der Library gelieferten Routen mit der gleichen
Matrix transformiert werden, die die Abweichung der räumlichen Lage des
Gebäudemodells von dessen initialer Lage beschreibt.

<a id="routenanzeige"></a>
### Anzeige der Route

Wie im vorherigen Abschnitt beschrieben, stellt die A* Library
berechnete Routen als Liste von Wegpunkten (`List<Vector3>`) zur
Verfügung. Für die Anzeige der Route reicht es also aus, einen normalen
`LineRenderer` in Unity zu verwenden, dessen anzuzeigende Positionen
immer dann auf auf die aktuellen Wegpunkte gesetzt werden, wenn die
`RichAI` Komponente mittels Event mitteilt, dass sich die Route geändert
hat.

![Line Renderer](./Images/line_renderer.png)

Damit die Route nur dann angezeigt wird, wenn die Wegfindung aktiv
ist, wird der `LineRenderer` gemeinsam mit dem Wegfindungs-Algorithmus
aktiviert, wenn der Benutzer mittels Sprachbefehl die Wegfindung
startet, und wieder deaktiviert, wenn die A* Library das Event
aussendet, dass die Zielposition erreicht wurde.

<a id="sonstiges"></a>
### Sonstige Implementierungsdetails

Im Abschnitt <a href="#positionsbestimmung">Positionsbestimmung</a> wurde
beschrieben, dass das Gebäudemodell nur als Kindelement an den
virtuellen Vuforia-Marker angehängt werden muss, um bei Erkennung des
zugehörigen physikalischen Markers richtig positioniert zu werden. Diese
Aussage ist zwar richtig, allerdings wäre es relativ mühsam, beim Testen
in Unity immer zuerst einen physikalischen Marker vor die Kamera halten
zu müssen, bevor das Gebäudemodell richtig positioniert wird.

Um das Testen in Unity zu erleichtern, wurde deshalb ein von Vuforia 
unabhängiges Interface `ITrackable` definiert, das grundsätzlich
beliebige "Trackables", d.h. Objekte, die als Marker verwendet werden
können, repräsentiert. Die zwei wichtigsten Implementierungen dieses
Interfaces sind:

-   `PhysicalTrackable`, das quasi als Adapter zwischen der
    `DefaultTrackableEventHandler` Klasse von Vuforia-Markern und dem
    `ITrackable` Interface fungiert, d.h. die Verwendung von physischen
    Markern ermöglich.
    
-   `VirtualTrackable`, das die Tracking-Events, die das `ITrackable`
    Interface definiert (`TrackingStarted`, `TrackingStopped`,
    `TrackingChanged`), dann auslöst, wenn ein virtuelles Objekt sich
    ins Zentrum des Kamera-sichtbaren bereichs bewegt bzw. wieder
    verlässt. `VirtualTrackable`s sind also Marker, die vollständig ohne
    physische Objekte funktionieren.

Die Applikation kann somit ganz im virtuellen Raum getestet werden, idem
einfach ein `VirtualTrackable` anstatt eines `PhysicalTrackable` für die
Positionierung des Gebäudemodells eingesetzt wird.



<a id="diskussion"></a>
## Diskussion der Ergebnisse

![Ergebnis](./Images/result.png)

Die Applikation funktioniert im Unity Editor und im HoloLens Emulator
von Microsoft sehr gut. Leider habe ich es aber bis zuletzt nicht
geschafft, die Applikation auch auf einer echten HoloLens zuverlässig
zum Laufen zu bringen.

Problem ist dabei nicht, dass die Funktionalität fehlerhafte Ergebnisse
liefert bzw. sich nicht wie gewünscht verhält (bis zu dem Punkt, an dem
ich das Testen hätte könnten, bin ich praktisch nie gekommen), sondern
dass trotz zahlreicher Versuche mit unterschiedlichen Unity und 
MRTK-Versionen (Unity 2018.3, 2018.4, 2019.1, MRTK 2.0 RC2.1, RC2,
Beta 2) bereits beim Starten, d.h. noch bevor irgendein Szenen-Inhalt
angezeigt wird, sehr zuverlässig Exceptions aufgetreten sind, die für
mich nicht nachvollziehbar waren. Zuletzt waren das Read Access
Violations in Camera.SetProjectionMatrix_Injected.

Größte Herausforderung während der Implementierung war ansonsten
der Abgleich der verschiedenen Koordinatensysteme (physisch, Unity,
HoloLens, A*), worauf in den vorangegangen Abschnitten ja bereits
detaillierter eingegangen wurde. Alle übrigen Funktionalitäten konnten
relativ problemlos auf Basis der verwendeten Frameworks bzw. Libraries
umgesetzt werden, sodass ich Vuforia, das A* Pathfinding Project und das
Mixed Reality Toolkit definitiv weiterempfehlen und auch in künftigen
Projekten ohne Bedenken wieder einsetzen würde.

Ein paar mögliche Erweiterungen der Applikation, deren Umsetzung sich in
der Verfügbaren Zeit nicht mehr ausgegangen ist, könnten z.B. sein:

-   Verwendung mehrerer Vuforia-Marker: um auch über längere Distanzen
    eine genaue Überlagerung des physischen Gebäudes und des virtuellen
    Gebäudemodells zu erreichen, könnten mehrere Marker im Gebäude
    platziert und das virtuelle Gebäudemodell bei deren Erkennung wieder
    neu ausgerichtet werden. Dadurch würde außerdem erreicht, dass
    Benutzer der Applikation nicht zuerst zum einzigen Marker im Gebäude
    gehen müssen, bevor deren Position richtig bestimmt werden kann.
    
-   Definition der erkannbaren Sprachbefehle auf Basis der vorhandenen
    Räume: aktuell sind die Sprachbefehle, die die Applikation kennt,
    fix definiert. Das Anlegen der erkannten Befehle ist somit mit einem
    manuellen Aufwand verbunden, der in größeren Modellen mit mehr
    Räumen rapide anwachsen würde. Wenn die Applikation automatisch
    Sprachbefehle für jeden verfügbaren Raum definieren, oder einen
    mit der Raum-Bezeichnung/-Nummer parametrisierbaren Befehl verwenden
    würde, ließe sich dieser manuelle Aufwand vollständig eliminieren.
    
-   Automatischer Download von Gebäudemodellen: Sehr cool wäre es, wenn
    die Applikation nicht nur ein fixes Gebäudemodell unterstützen
    würde, sondern z.B. in den verwendeten Markern eine Server-URL oder
    ähnliches codiert wäre, von der die Applikation das Modell des
    zum Marker gehörenden Gebäudes herunterladen könnte. Klarerweise
    müsste die HoloLens dafür aber auch mit dem Internet bzw. dem
    Netzwerk verbunden sein, in dem sich der Server befindet, was eine
    nicht zu unterschätzende Einschränkung ist und abhängig vom
    konkreten Einsatzbereich vermutlich nicht immer garantiert werden
    kann.
    
Insbesondere was UI/UX angeht gibt as darüber hinaus natürlich auch noch
sehr viel Verbesserungspotenzial.
