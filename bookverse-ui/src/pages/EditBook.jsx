import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../api/axiosInstance';

const EditBook = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [book, setBook] = useState({
    title: '',
    author: '',
    genre: '',
    isbn: '',
    rating: 1,
    isAvailable: true,
  });
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchBook = async () => {
      try {
        const response = await api.get(`/Books/${id}`);
        setBook(response.data.data);
      } catch (err) {
        console.error(err);
        setError('Failed to fetch book');
      }
    };
    fetchBook();
  }, [id]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setBook((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    try {
      await api.put(`/Books/${id}`, book);
      navigate('/books');
    } catch (err) {
      console.error(err);
      setError('Update failed');
    }
  };

  return (
    <div className="p-6 max-w-xl mx-auto">
      <h2 className="text-2xl font-bold mb-4 text-center text-blue-700">✏️ Edit Book</h2>

      <form onSubmit={handleSubmit} className="bg-white p-6 rounded shadow space-y-4">
        {['title', 'author', 'genre', 'isbn'].map((field) => (
          <input
            key={field}
            name={field}
            value={book[field]}
            onChange={handleChange}
            placeholder={field.charAt(0).toUpperCase() + field.slice(1)}
            className="w-full p-2 border rounded"
            required
          />
        ))}

        <div className="flex items-center gap-4">
          <label>⭐ Rating</label>
          <input
            type="number"
            name="rating"
            value={book.rating}
            onChange={(e) =>
                setBook((prev) => ({
                  ...prev,
                  rating: Number(e.target.value), 
                })
            )}
            min="1"
            max="5"
            className="p-2 border rounded w-20"
          />
        </div>

        <div className="flex items-center gap-4">
          <label>
            <input
              type="checkbox"
              name="isAvailable"
              checked={book.isAvailable}
              onChange={handleChange}
            />
            Available
          </label>
        </div>

        {error && <p className="text-red-500">{error}</p>}

        <button type="submit" className="w-full bg-green-600 text-white p-2 rounded hover:bg-green-700">
          Save Changes
        </button>
      </form>
    </div>
  );
};

export default EditBook;
