import React, { useState } from 'react';
import axios from '../api/axiosInstance';
import { useNavigate } from 'react-router-dom';

const AddBook = () => {
  const [formData, setFormData] = useState({
    title: '',
    author: '',
    genre: '',
    isbn: '',
    rating: '',
    isAvailable: true,
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value,
    });
  };

  const validateForm = () => {
    const { title, author, genre, isbn, rating } = formData;
    if (!title || !author || !genre || !isbn || !rating) {
      setError('All fields are required.');
      return false;
    }
    if (isbn.length < 10) {
      setError('ISBN must be at least 10 characters.');
      return false;
    }
    if (rating < 1 || rating > 5) {
      setError('Rating must be between 1 and 5.');
      return false;
    }
    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (!validateForm()) return;

    try {
      await axios.post('/Books', formData);
      setSuccess('✅ Book added successfully!');
      setFormData({ title: '', author: '', genre: '', isbn: '', rating: '', isAvailable: true });

      setTimeout(() => navigate('/books'), 1000); // redirect after success
    } catch (err) {
      console.error(err);
      setError('Something went wrong while adding the book.');
    }
  };

  return (
    <div className="max-w-md mx-auto mt-8 p-6 bg-white rounded-lg shadow-md">
      <h2 className="text-xl font-semibold mb-4">➕ Add New Book</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
        {['title', 'author', 'genre', 'isbn', 'rating'].map((field) => (
          <input
            key={field}
            type={field === 'rating' ? 'number' : 'text'}
            name={field}
            placeholder={field[0].toUpperCase() + field.slice(1)}
            className="w-full border px-3 py-2 rounded"
            value={formData[field]}
            onChange={handleChange}
            required
          />
        ))}
        <label className="flex items-center space-x-2">
          <input
            type="checkbox"
            name="isAvailable"
            checked={formData.isAvailable}
            onChange={handleChange}
          />
          <span>Available</span>
        </label>
        <button
          type="submit"
          className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded"
        >
          Add Book
        </button>
        {error && <p className="text-red-600 text-sm">{error}</p>}
        {success && <p className="text-green-600 text-sm">{success}</p>}
      </form>
    </div>
  );
};

export default AddBook;
