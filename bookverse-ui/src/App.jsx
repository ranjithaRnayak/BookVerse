import { useState, React } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import { Routes, Route } from 'react-router-dom';
import Login from './pages/Login';
import BookList from './pages/BookList';
import AddBook from './pages/AddBook';
import EditBook from './pages/EditBook';
import Navbar from './components/Navbar';
import PrivateRoute from './routes/PrivateRoute';

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <Navbar />
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route
          path="/books"
          element={
            <PrivateRoute>
              <BookList />
            </PrivateRoute>
          }
        />
        <Route
          path="/add-book"
          element={
            <PrivateRoute>
              <AddBook />
            </PrivateRoute>
          }
        />
         <Route
          path="/edit-book/:id"
          element={
            <PrivateRoute>
              <EditBook />
            </PrivateRoute>
          }
        />
      </Routes>

      
      <div height="4em">
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
    </>
  )
}

export default App
