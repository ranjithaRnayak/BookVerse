import React from 'react';
import { useNavigate } from 'react-router-dom';

const Navbar = () => {
  const navigate = useNavigate();
  const username = localStorage.getItem('username'); // We can store username on login
  const token = localStorage.getItem('token');

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    navigate('/login');
  };

  return (
    <nav className="bg-blue-700 text-white px-6 py-3 flex justify-between items-center">
      <h1 className="text-lg font-bold" onClick={() => navigate('/login')}>📚 BookVerse</h1>
      {token && (
        <div className="flex gap-4 items-center">
          <span className="text-sm">Hi, {username || 'User'}</span>
          <button
            onClick={handleLogout}
            className="bg-red-600 px-3 py-1 rounded hover:bg-red-700 text-sm"
          >
            Logout
          </button>
        </div>
      )}
    </nav>
  );
};

export default Navbar;
