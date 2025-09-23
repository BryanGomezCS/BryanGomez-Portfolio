#include <iostream>
#include "bst.h"

using namespace std;

int compare(const int &a, const int &b)
{
    return (a < b) ? -1 : (a > b) ? 1
                                  : 0;
}

void printNode(const int &item)
{
    cout << item << " ";
}

int main()
{
    // Making a BinarySearchTree with an integer data
    BinarySearchTree<int> bst(compare);

    // inserting elements in a way that creates an unbalanced tree
    cout << "Inserting elements to create an unbalanced tree..." << endl;
    bst.insert(10); // root
    bst.insert(5);  // left child
    bst.insert(15); // right child
    bst.insert(2);  // left child of 5
    bst.insert(7);  // right child of 5
    bst.insert(12); // left child of 15
    bst.insert(20); // right child of 15
    bst.insert(1);  // Adding more depth to the left side
    bst.insert(3);

    // Displaying just to make sure inOrder works
    cout << "\nTree before balancing: ";
    bst.inOrder(printNode);
    cout << endl;

    // Balancing the tree using the DSW algorithm
    cout << "\nBalancing the tree...\n";
    bst.balance();
    cout << "\nNew root after balancing: " << bst.getRoot()->data << endl;

    // Printing again after balancing
    cout << "\nTree after balancing: ";
    bst.inOrder(printNode);
    cout << endl;


    

    return 0;
}

