/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect } from "react";
import { useTodoItemStore } from "./todoItemStore";
import { Form, Input, Modal, notification } from "antd";
import { TodoItemService } from "./todoItemService";

interface TodoItemModalProps {
  visible: boolean;
  onClose: () => void;
}

export const TodoItemEdit: React.FC<TodoItemModalProps> = ({
  visible,
  onClose,
}) => {
  const { editTodoItem, setTodoItemEdit } = useTodoItemStore();
  const [form] = Form.useForm();

  useEffect(() => {
    if (editTodoItem?.id) {
      form.setFieldsValue(editTodoItem);
    }
  }, [editTodoItem, form]);

  const handleSaveTodoItem = () => {
    form.validateFields().then(async (values) => {
      values.id = editTodoItem?.id;

      try {
        await TodoItemService.updateTodoItem(values);
        notification.success({ message: "Operation successful!" });
        setTodoItemEdit(null);
        onClose();
      } catch (error: any) {
        console.error("Error updating todo item:", error);
        notification.error({ message: "Failed to update todo item!" });
      }
    });
  };

  return (
    <Modal
      title={"Edit Todo Item"}
      open={visible}
      onCancel={() => {
        setTodoItemEdit(null);
        onClose();
      }}
      onOk={handleSaveTodoItem}
    >
      <Form form={form} layout="vertical">
        <Form.Item
          label="Todo Item Name"
          name="title"
          rules={[
            { required: true, message: "Please enter the todo item name!" },
          ]}
        >
          <Input placeholder="Enter todo item name" />
        </Form.Item>
      </Form>
    </Modal>
  );
};
