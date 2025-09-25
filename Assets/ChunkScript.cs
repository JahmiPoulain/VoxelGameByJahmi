using UnityEngine;
using UnityEngine.UIElements;

public class ChunkScript : MonoBehaviour
{
    // /!\/!\/!\/!\/!\ Je ne connais pas assez les ordinateurs pour savoir si mes optimisations de RAM sont utiles ou contre productives /!\/!\/!\/!\/!\

    // /!\/!\/!\/!\/!\ J'ai commencer la reflexion et trouver l'idee des formules de traduction Vector3 = Index avant d'arriver a Rubika
    // /!\/!\/!\/!\/!\ Je ne sait pas suffisement manipuler les mesh donc je genere des gameobjects de cubes a la place d'un seul grand mesh optimise
    // /!\/!\/!\/!\/!\ mes optimisations ne fonctionnent pas avec les gameobjects cube car ils ont un component transform qui anulent mes optimisations
    /*
    Le but de mon moteur voxel est de stoquer un minimum d'informations dans la RAM et de compensser se manque d'info par des calculs en runtime qui seront fait uniquement quand il le faut
    Pour afficher un bloc a l'ecran il faut savoir ou se bloc doit apparaitre mais stoquer un Vector3 c'est caca boudin pour la RAM car c'est 3 ints
    Pour eviter le Vector3 je genere le monde dans un Array dans lequel chaque index correspond à un Vector 3 dans le monde car cet array est remplit via un algorythme qui lit des coordonées x,y,z dans un ordre precis
    Cette relation entre l'index de l'array et les coordonee dans le monde permet de calculer facilement l'un à partir de l'autre avec des formules
    Donc en principe on peut creer un mesh optimiser pour representer le monde a partir d'un array sans avoir a stoquer les coordonees de chaques blocs, a la place on stoque juste d'identite du bloc avec un simple int
    Pour l'instant l'identite d'un bloc est juste vide(0) ou plein(1)
     */
    [Header("Dimensions du chunk")]
    public int chunkSize;
    public int chunkHeight;
    [Header("Arrays")]
    public int[] binaryChunkGrid;
    public int currentBlockIndexInArray;
    public Vector3 currentBlockCoordinate;
    public int indexOfClickedBlock;
    public bool setFalse;

    public GameObject[] cubeGameobjectArray;
    [Header("Tests et modes")]
    public bool MeshGenerationOrInstantiateCube;
    public GameObject cubeGameobject;
    public bool randomChunk;

    [Header("Bruit de Perlin")]
    //public Texture2D noiseTex;
    public float xOrg;
    public float yOrg;
    void Start()
    {
        binaryChunkGrid = new int[chunkSize * chunkSize * chunkHeight]; // l'array est assez grand pour contenir toute l'information du chunk  
        cubeGameobjectArray = new GameObject[chunkSize * chunkSize * chunkHeight];
        CreateChunkArray();
    }

    // Cet algo remplit l'array du chunk d'une certaine façon
    // si il est remplit autrement les formules de traduction "Vector3 = Index Array" ne fonctionnent plus
    void CreateChunkArray() // Algorythme qui sert à remplir l'array d'un chunk
    {
        for (int y = 0; y < chunkHeight; y++) // pour chaque coordonées y
        {
            currentBlockCoordinate.y = y;

            for (int z = 0; z < chunkSize; z++) // pour chaque coordonées z
            {
                currentBlockCoordinate.z = z;

                for (int x = 0; x < chunkSize; x++) // pour chaque coordonées x
                {
                    currentBlockCoordinate.x = x;
               
                    float sample = Mathf.PerlinNoise(x, z);
                    //Debug.Log(sample);

                    if (randomChunk)
                    {
                        binaryChunkGrid[currentBlockIndexInArray] = Random.Range(0, 2);
                        if (binaryChunkGrid[currentBlockIndexInArray] == 1)
                        {
                            if (MeshGenerationOrInstantiateCube) // pour changer de l'instantiation de gameobject à la manipulation de maillage (mesh)
                            {

                            }
                            else
                            {
                                setFalse = false;
                                InstantiateCube();
                            }
                        }
                        else
                        {
                            if (MeshGenerationOrInstantiateCube)
                            {

                            }
                            else
                            {
                                setFalse = true;
                                InstantiateCube();
                            }
                        }
                    }
                    else
                    {
                        if (y < sample)
                        {
                            binaryChunkGrid[currentBlockIndexInArray] = 1;
                            if (MeshGenerationOrInstantiateCube)
                            {

                            }
                            else
                            {
                                InstantiateCube();
                            }
                        }
                        else
                        {
                            if (MeshGenerationOrInstantiateCube)
                            {

                            }
                            else
                            {
                                setFalse = true;
                                InstantiateCube();
                            }
                        }
                    }                 
                    currentBlockIndexInArray++;
                    //Instantiate(nextBlockToInstantiate, new Vector3(x + currentChunkCoordinate.x, y, z + currentChunkCoordinate.z), transform.rotation);
                }
            }
        }
    }

    void InstantiateCube() // On instantie un GameObject cube en fonction des données dans l'array du chunk
    {
        var currentCubeGameobject = Instantiate(cubeGameobject, currentBlockCoordinate + new Vector3(0.5f, 0.5f, 0.5f), transform.rotation); // on instantie le cube avec un décalage de 0.5 pour l'aligner avec la grille, sinon il ne peut pas etre detecté et traduit par les formules
        currentCubeGameobject.GetComponent<CubeGameobjectScript>().indexCube = currentBlockIndexInArray; // sert uniquement à débugger
        cubeGameobjectArray[currentBlockIndexInArray] = currentCubeGameobject;
        Debug.Log("Bloc index " + currentBlockIndexInArray + " corresponds to " + currentBlockCoordinate);
        if (setFalse)
        {
            currentCubeGameobject.SetActive(false);

        }
        
        //Debug.Log(2);
        //Debug.Log(cubeGameobjectArray[currentBlockIndexInArray]);
    }

    public void BreakingBlocks(Vector3 realLeftClickBlockPosition) // on recoit les coordonées du click gauche du joueur en Vector3
    {
        //Debug.Log(realLeftClickBlockPosition);
        var Vector3ToIndex = ((realLeftClickBlockPosition.x) + (chunkSize * realLeftClickBlockPosition.z) + (realLeftClickBlockPosition.y * chunkSize * chunkSize)); // on traduit le Vector3 vers son index correspondant // x + (z * L) + (y * L²) = Index
        //Debug.Log(Vector3ToIndex);
        indexOfClickedBlock = (int)Vector3ToIndex;
        if (indexOfClickedBlock < binaryChunkGrid.Length) // si les coordonées sont correspondent à un index
        {
            binaryChunkGrid[indexOfClickedBlock] = 0; // l'index correspondant = 0
            Debug.Log("Vector3: " + realLeftClickBlockPosition + " = Index " +indexOfClickedBlock);
            UpdateChunk(); // mettre à jour le maillage / les gameobjects
        }
        //cubeGameobjectArray[indexOfClickedBlock] = null;
        //cubeGameobjectArray[indexOfClickedBlock].SetActive(false);
    }

    public void PlacingBlocks(Vector3 xyz) // on recoit les coordonées du click droit +- 1 du joueur en Vector3
    {
        var Vector3ToIndex = ((xyz.z * chunkSize) + xyz.x) + (chunkSize * chunkSize * xyz.y); // traduire Vector3 --> Index
        indexOfClickedBlock = (int)Vector3ToIndex;
        if (indexOfClickedBlock < binaryChunkGrid.Length)
        {
            binaryChunkGrid[indexOfClickedBlock] = 1; // l'index correspondant = 1
            Debug.Log("Vector3: " + xyz + " =  Index " + indexOfClickedBlock);
            UpdateChunk(); // mettre à jour le maillage / les gameobjects
            //Debug.Log(indexOfClickedBlock);
        }
    }
    void UpdateChunk() // met a jour l'array des gameobject pour l'aligner avec l'array binaire
    {
        if (binaryChunkGrid[indexOfClickedBlock] == 1) // Si il doit y avoir un bloc on active le cube sinon on le desactive
        {
            cubeGameobjectArray[indexOfClickedBlock].SetActive(true);
        }
        else
        {
            cubeGameobjectArray[indexOfClickedBlock].SetActive(false);
        }
    }


    // /!\/!\/!\/!\/!\ NE SERT PAS POUR L'INSTANT, PAS TEST /!\/!\/!\/!\/!\/!\
    // j'ai pas prouver les formules !!!!!
    // servia à generer les vertices aux bonnes coordonées plus tard
    public void TranslateIndexToVector3xyz() // Traduit Index Array --> Vector3 
    {
        // placing blocks
        var x = indexOfClickedBlock % chunkSize; //indexOfClickedBlock - chunkSize * Zint; (int - L) / L
        var Xint = (int)x;

        var y = indexOfClickedBlock / (chunkSize * chunkSize); //((indexOfClickedBlock - (chunkSize * chunkSize)) - (chunkSize - Zint)) / (chunkSize * chunkSize);
        var Yint = (int)y;

        var z = indexOfClickedBlock / chunkSize - (chunkSize * Yint); //indexOfClickedBlock / chunkSize; 
        var Zint = (int)z;

        currentBlockCoordinate = new Vector3(Xint, Yint, Zint);
        Debug.Log(currentBlockCoordinate);
        // z = int / L = float to int = z
        // x = int - L * z int
        // y = int / L�
    }
}
