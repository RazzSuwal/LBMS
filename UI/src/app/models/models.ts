export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  mobileNumber: string;
  password: string;
  userType: UserType;
  accountStatus: AccountStatus;
  createdOn: string;
}

export enum AccountStatus {
  UNAPROOVED,
  ACTIVE,
  BLOCKED,
}

export enum UserType {
  ADMIN,
  STUDENT,
}

export interface BookCategory {
  id: number;
  category: string;
  subCategory: string;
}

export interface Book {
  id: number;
  title: string;
  author: string;
  price: number;
  ordered: boolean;
  bookCategoryId: number;
  bookCategory: BookCategory;
}
export interface Books {
  title: string;
  author: string;
  price: number;
  ordered: boolean;
  bookCategoryId: number;
}
export interface Booked {
  id: number;
  title: string;
  author: string;
  price: number;
  ordered: boolean;
  bookCategoryId: number;
}
export interface BooksByCategory {
  bookCategoryId: number;
  category: string;
  subCategory: string;
  books: Book[];
}

export interface Order {
  id: number;
  userId: number;
  userName: string | null;
  bookId: number;
  bookTitle: string;
  orderDate: string;
  returned: boolean;
  returnDate: string | null;
  finePaid: number;
}

export interface Customer {

  DocumentNo: number;
  CustomerName: string;
  MobileNumber: string;
}

export interface Product {
  ProductName: string;
  Quantity: number;
  Rate: number;
  Amount: number;
  CustomerId: number;
}

export interface Billing {
  Discount: number;
  GrandTotal: number;
  CustomerId: number;
}
export interface BillingVM {
  CustomerId: number;
  DocumentNo: number;
  CustomerName: string;
  MobileNumber: string;
  Products: ProductVM[]; // Array of products
  BillingId: number;
  Discount: number;
  GrandTotal: number;
}

export interface ProductVM {
  ProductName: string;
  Quantity: number;
  Rate: number;
  Amount: number;
}


export interface BooksWithPagination{
  id: number;
  title: string;
  author: string;
  price: number;
  ordered: boolean;
  category: string;
  subCategory: string;
}


