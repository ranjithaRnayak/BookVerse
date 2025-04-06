import React, { useEffect, useState } from 'react';
import api from '../api/axiosInstance';
import { Link } from 'react-router-dom';
import useDebounce from '../hooks/useDebounce';
import { getUserRole } from '../utils/authUtils';


const BookList = () => {
    const [allBooks, setAllBooks] = useState([]);
    const [rawSearchInput, setRawSearchInput] = useState('');
    const search = useDebounce(rawSearchInput, 300);
    const [page, setPage] = useState(1);
    const [error, setError] = useState('');
    const pageSize = 5;
    const userRole = getUserRole();

    const filteredBooks = allBooks.filter(book =>
        book.title.toLowerCase().includes(search.toLowerCase()) ||
        book.author.toLowerCase().includes(search.toLowerCase())
    );

    const paginatedBooks = filteredBooks.slice((page - 1) * pageSize, page * pageSize);
    const totalPages = Math.ceil(filteredBooks.length / pageSize);

    const fetchBooks = async () => {
        try {
            const response = await api.get(`/Books`);
            const allBooks = response?.data?.data || [];
            setAllBooks(allBooks);
            // setTotalCount(allBooks.length);
        } catch (err) {
            console.error(err);
            setError('Failed to load books');
        }
    };
    const handleDelete = async (id) => {
        const confirm = window.confirm('Are you sure you want to delete this book?');
        if (!confirm) return;
      
        try {
          await api.delete(`/Books/${id}`);
          setAllBooks((prev) => prev.filter((book) => book.id !== id));
        } catch (err) {
          console.error(err);
          alert('Failed to delete the book.');
        }
      };
      

    useEffect(() => {
        fetchBooks();
    }, []);
    useEffect(() => {
        setPage(1); // Reset to first page when search changes
    }, [search]);

    return (
        <div className="p-6">
            <h2 className="text-3xl font-extrabold mb-6 text-center text-blue-700">📚 Book List</h2>

            <div className="mb-6 flex flex-col md:flex-row justify-between items-center gap-4">
                <input
                    type="text"
                    placeholder="🔍 Search by title/author"
                    value={rawSearchInput}
                    onChange={(e) => setRawSearchInput(e.target.value)}
                    className="border p-2 rounded w-full md:w-1/3 shadow-sm"
                />
                {userRole === 'Admin' || userRole === "Intern" && (
                    <Link
                        to="/add-book"
                        className="bg-green-600 text-white px-5 py-2 rounded hover:bg-green-700 text-sm font-semibold shadow"
                    >
                        ➕ Add Book
                    </Link>
                )}
            </div>

            {error && <p className="text-red-600 text-center mb-4">{error}</p>}

            <div className="overflow-x-auto shadow rounded-lg">
                <table className="min-w-full bg-white border">
                    {paginatedBooks.length > 0 && (
                        <thead>
                            <tr className="bg-blue-100 text-blue-700 text-left">
                                <th className="p-3 border">📖 Title</th>
                                <th className="p-3 border">✍️ Author</th>
                                <th className="p-3 border">🏷️ Genre</th>
                                <th className="p-3 border">⭐ Rating</th>
                                {userRole === 'Admin' && <th className="p-3 border text-center"> ✏️/ 🗑️Actions</th>}
                            </tr>
                        </thead>
                    )}
                    <tbody>
                        {paginatedBooks.map((book) => (
                            <tr key={book.id} className="hover:bg-gray-50">
                                <td className="p-2 border">{book.title}</td>
                                <td className="p-2 border">{book.author}</td>
                                <td className="p-2 border">{book.genre}</td>
                                <td className="p-2 border">{book.rating}</td>
                                {userRole === 'Admin' && (
                                    <td className="p-2 border text-blue-600 text-sm">
                                        <Link to={`/edit-book/${book.id}`} className="hover:underline">✏️ Edit</Link>
                                    </td>
                                )}
                                {userRole === 'Admin' && (
                                    <td className="p-2 border text-center">
                                        <button
                                            onClick={() => handleDelete(book.id)}
                                            className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600 text-xs"
                                        >
                                            🗑️ Delete
                                        </button>
                                    </td>
                                )}
                            </tr>
                        ))}
                    </tbody>
                    {paginatedBooks.length === 0 && (
                        <tbody>
                            <tr>
                                <td colSpan="4" className="text-center p-4 text-gray-500">
                                    No books found for "{rawSearchInput}" 😕
                                </td>
                            </tr>
                        </tbody>
                    )}
                </table>
            </div>

            <div className="mt-6 flex justify-center items-center gap-4">
                <button
                    onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
                    disabled={page === 1}
                    className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50"
                >
                    ⬅️ Previous
                </button>

                <span className="text-sm font-semibold">Page {page} of {totalPages}</span>

                <button
                    onClick={() => setPage((prev) => Math.min(prev + 1, totalPages))}
                    disabled={page >= totalPages}
                    className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50"
                >
                    Next ➡️
                </button>
            </div>
        </div>
    );
};

export default BookList;
