#include <stdio.h>
#include <string.h>

#define MAX_BOOKS 100
#define FILE_NAME "library.dat"

typedef struct
{
    int id;
    char title[100];
    char author[100];
    int status;
} Book;

Book library[MAX_BOOKS];
int totalBooks = 0;

void addBook();
void displayBooks();
void searchBook();
void borrowReturnBook();
void deleteBook();
void saveToFile();
void loadFromFile();

int main()
{
    int choice;

    loadFromFile();

    while (1)
    {
        printf("\nWelcome to Library Management System\n");
        printf("What would like to do today?\n");
        printf("1. Add Book\n");
        printf("2. Display All Books\n");
        printf("3. Search Book\n");
        printf("4. Borrow/Return Book\n");
        printf("5. Delete Book\n");
        printf("6. Exit\n");
        printf("Enter your choice: ");
        scanf("%d", &choice);

        switch (choice)
        {
            case 1: addBook(); break;
            case 2: displayBooks(); break;
            case 3: searchBook(); break;
            case 4: borrowReturnBook(); break;
            case 5: deleteBook(); break;
            case 6: saveToFile(); return 0;
            default: printf("Invalid choice! Please try again.\n");
        }
    }
    return 0;
}

void addBook()
{
    if (totalBooks >= MAX_BOOKS)
    {
        printf("Library is full! Cannot add more books");
        return;
    }
    Book newBook;

    printf("Enter Book ID: ");
    scanf("%d", &newBook.id);
    getchar();

    printf("Enter Book Title: ");
    fgets(newBook.title, 100, stdin);
    newBook.title[strcspn(newBook.title, "\n")] = 0;

    printf("Enter Book Author: ");
    fgets(newBook.author, 100, stdin);
    newBook.author[strcspn(newBook.author, "\n")] = 0;

    newBook.status = 1;

    library[totalBooks++] = newBook;
    printf("Book added successfully\n");
}

void displayBooks()
{
    if (totalBooks == 0)
    {
        printf("No book available in the library");
        return;
    }
    printf("\nID\tTitle\tAuthor\tStatus\n");
    printf("-----------------------------------\n");
    for (int i = 0; i < totalBooks; i++)
    {
        printf("%d\t%s\t%s\t%s\n",
               library[i].id,
               library[i].title,
               library[i].author,
               library[i].status == 1 ? "Available" : "Borrowed");
    }
}

void searchBook()
{
    int id, found = 0;
    printf("Enter Book ID to search: ");
    scanf("%d", &id);

    for (int i = 0; i < totalBooks; i++)
    {
        if (library[i].id == id)
        {
            printf("\nBook found\n");
            printf("ID: %d\nTitle: %s\nAuthor: %s\nStatus: %s\n",
                   library[i].id,
                   library[i].title,
                   library[i].author,
                   library[i].status == 1 ? "Available" : "Borrowed");
            found = 1;
            break;
        }
    }
    if(!found) {
        printf("Book not found!\n");
    }
}

void borrowReturnBook() {
    int id, found = 0;
    printf("Enter Book ID to borrow/return: ");
    scanf("%d", &id);

    for (int i = 0; i < totalBooks; i++)
    {
        if(library[i].id == id) {
            found = 1;
            if(library[i].status == 1) {
                library[i].status = 0;
                printf("Book borrowed successfully!");
            } else {
                library[i].status = 1;
                printf("Book returned successfully");
            }
            break;
        }
    }
    if(!found) {
        printf("Book not found!\n");
    }
}

void deleteBook() {
    int id, found = 0;
    printf("Enter Book ID to delete: ");
    scanf("%d", &id);

    for (int i = 0; i < totalBooks; i++)
    {
        if(library[i].id == id) {
            found = 1;
            for (int j = i; j < totalBooks; j++)
            {
              library[j] = library[j + 1];
            }
            totalBooks--;
            printf("Book deleted sucessfully!\n");
            break;
        }
    }
    if(!found) {
        printf("Book not found !\n");
    }
}

void saveToFile() {
    FILE *file = fopen(FILE_NAME, "wb");
    if(file == NULL) {
        printf("Error opening file for storing data");
        return;
    }
    fwrite(&totalBooks, sizeof(int), 1, file);
    fwrite(library, sizeof(Book), totalBooks, file);
    fclose(file);
    printf("Library data saved successfully!\n");
}

void loadFromFile() {
    FILE *file = fopen(FILE_NAME, "rb");
    if(file == NULL) {
        printf("No previous data found.\n");
        return;
    }
    fread(&totalBooks, sizeof(int), 1, file);
    fread(library, sizeof(Book), totalBooks, file);
    fclose(file);
}