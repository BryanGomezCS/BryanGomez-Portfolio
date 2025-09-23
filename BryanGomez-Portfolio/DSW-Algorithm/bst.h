#pragma once

#include <string>
#include <sstream>
#include <cmath>

using std::string;
using std::stringstream;

class Exception
{
protected:
    string message;
    int errorNumber;

public:
    Exception(int eNo, string msg) : errorNumber(eNo), message(msg) {}
    virtual string toString()
    {
        stringstream sstream;
        sstream << "Exception: " << errorNumber << " ERROR: " << message;
        return sstream.str();
    }
};

class DuplicateItemException : public Exception
{
public:
    DuplicateItemException(int eNo, string msg) : Exception(eNo, msg) {}
    string toString() const
    {
        stringstream sstream;
        sstream << "DuplicateItemException: " << errorNumber << " ERROR: " << message;
        return sstream.str();
    }
};

class TreeEmptyException : public Exception
{
public:
    TreeEmptyException(int eNo, string msg) : Exception(eNo, msg) {}
    string toString() const
    {
        stringstream sstream;
        sstream << "TreeEmptyException: " << errorNumber << " ERROR: " << message;
        return sstream.str();
    }
};

class ItemNotFoundException : public Exception
{
public:
    ItemNotFoundException(int eNo, string msg) : Exception(eNo, msg) {}
    string toString() const
    {
        stringstream sstream;

        sstream << "ItemNotFoundException: " << errorNumber << " ERROR: " << message;
        return sstream.str();
    }
};

template <typename DATA_TYPE>
class BinarySearchTree
{
    // By placing this class definition inside the private section of the tree class,
    // it is only accessible to the tree class
    class BinaryTreeNode
    {
    public:
        DATA_TYPE data;
        BinaryTreeNode *parent;
        BinaryTreeNode *left;
        BinaryTreeNode *right;

        BinaryTreeNode() { parent = left = right = nullptr; }
    };

    BinaryTreeNode *root;
    int nodeCount;
    // This is the member variable that will be used to hold the comparison function
    int (*compare)(const DATA_TYPE &item1, const DATA_TYPE &item2);
    BinaryTreeNode *findParentOrDuplicate(const DATA_TYPE &item);
    void privateInOrder(BinaryTreeNode *node, void (*visit)(const DATA_TYPE &item))
    {
        if (!node)
            return;

        privateInOrder(node->left, visit);
        visit(node->data);
        privateInOrder(node->right, visit);
    }

    void postOrderDelete(BinaryTreeNode *node)
    {
        if (!node)
            return;

        postOrderDelete(node->left);
        postOrderDelete(node->right);
        delete node;
    }

    void rotateRight(BinaryTreeNode *node);
    void rotateLeft(BinaryTreeNode *node);

public:
    /**
     * The constructor takes as a parameter the comparison function that will be
     * used to determine the structure of the tree
     */
    BinarySearchTree(int (*cmp)(const DATA_TYPE &item1, const DATA_TYPE &item2));
    ~BinarySearchTree();
    void insert(DATA_TYPE item);
    void remove(const DATA_TYPE &item);
    DATA_TYPE search(const DATA_TYPE &item);
    int size()
    {
        return nodeCount;
    }

    void inOrder(void (*visit)(const DATA_TYPE &item))
    {
        privateInOrder(root, visit);
    }
    // Adding the rotation functions to be public
    void pubRotateRight(const DATA_TYPE &item)
    {
        BinaryTreeNode *searchResult = findParentOrDuplicate(item);
        if (!searchResult || compare(searchResult->data, item))
        {
            throw ItemNotFoundException(__LINE__, "Item was not found");
        }
        rotateRight(searchResult);
    }

    void pubRotateLeft(const DATA_TYPE &item)
    {
        BinaryTreeNode *searchResult = findParentOrDuplicate(item);
        if (!searchResult || compare(searchResult->data, item))
        {
            throw ItemNotFoundException(__LINE__, "Item was not found");
        }
        rotateLeft(searchResult);
    }

    void balance();

    BinaryTreeNode *getRoot();
};

// Public methods
template <typename DATA_TYPE>
BinarySearchTree<DATA_TYPE>::BinarySearchTree(
    int (*cmp)(const DATA_TYPE &item1, const DATA_TYPE &item2))
{
    compare = cmp;
    nodeCount = 0;
    root = nullptr;
}

template <typename DATA_TYPE>
BinarySearchTree<DATA_TYPE>::~BinarySearchTree()
{
    postOrderDelete(root);
}

template <typename DATA_TYPE>
void BinarySearchTree<DATA_TYPE>::insert(DATA_TYPE item)
{
    if (!root) // Insert into an empty tree
    {
        root = new BinaryTreeNode();
        root->data = item;
        nodeCount++;
        return;
    }

    // Find the parent node, or identify a duplicate entry
    BinaryTreeNode *searchNode = findParentOrDuplicate(item);
    if (!compare(searchNode->data, item)) // Check to see if the item already exists
    {
        // Duplicate item detected, throw an exception
        DuplicateItemException exception(__LINE__, "Duplicate item detected. Unable to insert");
        throw exception;
    }

    // Item didn't exist, we can proceed as normal. searchNode now points to the parent node

    // Create new node
    BinaryTreeNode *node = new BinaryTreeNode();
    node->data = item;

    // Link the parent
    node->parent = searchNode;

    // Determine if the node will be a left or right child
    // Attach the node to the appropriate side
    if (compare(searchNode->data, item) == 1) // left child
        searchNode->left = node;
    else
        searchNode->right = node;

    // Increment the node counter
    nodeCount++;
}

template <typename DATA_TYPE>
void BinarySearchTree<DATA_TYPE>::remove(const DATA_TYPE &item)
{
    // Find the item to remove
    BinaryTreeNode *searchResult = findParentOrDuplicate(item);
    if (!compare(searchResult->data, item))
    {
        // Throw item not found exception
        throw ItemNotFoundException(__LINE__, "Item was not found");
    }

    // Check to see if it is a simple or hard case
    if (searchResult->left && searchResult->right)
    {
        // If hard
        //  reduce to a simple case, then reset the pointer
        //  Find the immediate predecessor
        BinaryTreeNode *current = searchResult->left;
        while (current->right)
            current = current->right;
        // Swap the data item
        DATA_TYPE tmp = searchResult->data;
        searchResult->data = current->data;
        current->data = tmp;

        // Set searchResult to the immediate predecessor
        searchResult = current;
    }

    BinaryTreeNode *child = searchResult->right ? searchResult->right : searchResult->left;
    BinaryTreeNode *parent = searchResult->parent;

    if (parent)
    {
        // I'm using a double pointer here rather than having a separate if/else for this.
        BinaryTreeNode **side = parent->right == searchResult ? &(parent->right) : &(parent->left);
        *side = child;
        if (child)
            child->parent = parent;
    }
    else
    {
        // Node was the root
        root = child;
    }

    delete searchResult;

    nodeCount--;
}

template <typename DATA_TYPE>
DATA_TYPE BinarySearchTree<DATA_TYPE>::search(const DATA_TYPE &item)
{
    BinaryTreeNode *searchResult = findParentOrDuplicate(item);
    if (!searchResult || compare(searchResult->data, item))
    {
        // Throw ItemNotFoundException
        throw ItemNotFoundException(__LINE__, "Item was not found");
    }

    return searchResult->data;
}

// Private functions
/**
 * This function searches the tree for the node that either contains a duplicate of the item,
 * or the node that would be the parent of the item.
 */
template <typename DATA_TYPE>
typename BinarySearchTree<DATA_TYPE>::BinaryTreeNode *BinarySearchTree<DATA_TYPE>::findParentOrDuplicate(const DATA_TYPE &item)
{
    BinaryTreeNode *current = root;
    BinaryTreeNode *parent = current;

    while (current)
    {
        parent = current;
        if (!compare(current->data, item))
            break;
        // Next, decide if we need to go left or right.
        if (compare(current->data, item) == 1) // Go left
            current = current->left;
        else // Go right. Duplicate is detected in the test of the while
            current = current->right;
    }

    return parent;
}

template <typename TYPE>
void BinarySearchTree<TYPE>::rotateRight(BinaryTreeNode *node)
{
    if (!node || !node->left)
        return; // Cannot rotate right if there is no left child

    BinaryTreeNode *parent = node->left;      // The left child becomes the new parent
    BinaryTreeNode *child = parent->right;    // The right subtree of the left child
    BinaryTreeNode *grandPapa = node->parent; // The parent of the current node

    // Updating the left child's right subtree to be the node's left subtree
    node->left = child;
    if (child)
        child->parent = node; // Update the parent of the moved subtree

    // Updating the parent of the left child
    parent->parent = grandPapa;

    // Updating the grandparent's child pointer
    if (!grandPapa)
        root = parent; // If the node is the root, update the root pointer
    else if (node == grandPapa->left)
        grandPapa->left = parent; // Update the parent's left child
    else
        grandPapa->right = parent; // Update the parent's right child

    // Updating the left child's right pointer and the node's parent pointer
    parent->right = node;
    node->parent = parent;
}

template <typename TYPE>
void BinarySearchTree<TYPE>::rotateLeft(BinaryTreeNode *node)
{
    // Implement this function

    BinaryTreeNode *parent = node->parent;

    if (!parent || node != parent->right)
        return;

    BinaryTreeNode *child = node->left;
    BinaryTreeNode *grandPapa = parent->parent;

    node->left = parent;
    parent->parent = node;

    if (grandPapa)
    {
        if (compare(grandPapa->data, node->data) == 1)
        {
            grandPapa->left = node;
        }
        else
        {
            grandPapa->right = node;
        }
    }
    else
    {
        root = node;
    }

    node->parent = grandPapa;

    parent->right = child;
    if (child)
    {
        child->parent = parent;
    }
}

template <typename DATA_TYPE>
void BinarySearchTree<DATA_TYPE>::balance()
{

    // Step 1: Creating the backbone (vine)
    BinaryTreeNode *node = root;
    while (node)
    {
        while (node->left)
        {
            rotateRight(node);
        }
        node = node->right;
    }

    // Step 2: Calculating the number of nodes (n) and the largest power of 2 less than or equal to n+1 (m)
    int n = nodeCount;
    int m = pow(2, floor(log2(n + 1))) - 1;

    std::cout << "Number of nodes: " << n << ", m: " << m << std::endl;

    // Step 3: Performing the first round of left rotations to reduce n - m nodes
    node = root;
    for (int i = 0; i < n - m; ++i)
    {
        if (!node || !node->right)
            break;

        rotateLeft(node->right);
        node = node->right;
    }

    // Step 4: Performing successive rounds of left rotations to balance the tree
    while (m > 1)
    {
        m /= 2;
        node = root;

        for (int i = 0; i < m; ++i)
        {
            if (!node || !node->right)
                break;

            rotateLeft(node->right);
            node = node->right;
        }
    }

    std::cout << "Tree balancing complete." << std::endl;
}

template <typename T>
typename BinarySearchTree<T>::BinaryTreeNode *BinarySearchTree<T>::getRoot()
{
    return root;
}
